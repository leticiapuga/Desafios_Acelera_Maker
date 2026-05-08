using Npgsql;
using ProjetoContaBancaria.Models;
using ProjetoContaBancaria.Repositories;
using ProjetoContaBancaria.Utils;
using System;
using System.Collections.Generic;

namespace ProjetoContaBancaria.Controllers
{
    // Controlador que implementa a interface de repositório e gerencia as contas.
    // Classe responsável por toda a lógica de persistência e operações das contas bancárias.
    public class ContaController : IContaRepository
    {
        // String de conexão com o banco de dados PostgreSQL.
        private readonly string _connectionString;

        // Construtor: busca a string de conexão do arquivo de configuração.
        public ContaController()
        {
            _connectionString = Configuracao.GetConnectionString("DefaultConnection");
        }

        // Busca uma conta pelo número no banco de dados.
        public Conta? ProcurarPorNumero(int numero)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return ProcurarPorNumero(numero, conn, null, false);
        }

        // Retorna todas as contas cadastradas no banco.
        public List<Conta> ListarTodas()
        {
            var contas = new List<Conta>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM contas ORDER BY numero", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                contas.Add(MapConta(reader));

            return contas;
        }

        // Cadastra uma nova conta no banco e retorna o número gerado.
        public int Cadastrar(Conta conta)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string sql = "INSERT INTO contas (agencia, tipo, titular, saldo, limite, aniversario) VALUES (@agencia, @tipo, @titular, @saldo, @limite, @aniversario) RETURNING numero;";
            using var cmd = new NpgsqlCommand(sql, conn);
            PreencherParametrosConta(cmd, conta);

            var numeroConta = cmd.ExecuteScalar();
            return Convert.ToInt32(numeroConta);
        }

        // Atualiza os dados de uma conta existente no banco.
        public void Atualizar(Conta conta)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            Atualizar(conta, conn, null);
        }

        // Remove uma conta do banco pelo número.
        public void Deletar(int numero)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM contas WHERE numero = @numero", conn);
            cmd.Parameters.AddWithValue("@numero", numero);
            cmd.ExecuteNonQuery();
        }

        // Realiza o saque em uma conta, se possível, e atualiza no banco.
        public void Sacar(int numero, float valor)
        {
            if (valor <= 0) return;

            var conta = ProcurarPorNumero(numero);
            if (conta != null && conta.Sacar(valor))
                Atualizar(conta);
        }

        // Realiza o depósito em uma conta e atualiza no banco.
        public void Depositar(int numero, float valor)
        {
            if (valor <= 0) return;

            var conta = ProcurarPorNumero(numero);
            if (conta != null)
            {
                conta.Depositar(valor);
                Atualizar(conta);
            }
        }

        // Realiza transferência entre duas contas com transação no PostgreSQL.
        // A transação garante que origem e destino sejam atualizadas juntas.
        // Se qualquer etapa falhar, nenhuma alteração é confirmada no banco.
        public void Transferir(int numeroOrigem, int numeroDestino, float valor)
        {
            if (numeroOrigem == numeroDestino || valor <= 0) return;

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                var origem = ProcurarPorNumero(numeroOrigem, conn, transaction, true);
                var destino = ProcurarPorNumero(numeroDestino, conn, transaction, true);

                if (origem == null || destino == null || !origem.Sacar(valor))
                {
                    transaction.Rollback();
                    return;
                }

                destino.Depositar(valor);

                Atualizar(origem, conn, transaction);
                Atualizar(destino, conn, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Número é gerado automaticamente pelo banco (SERIAL).
        public int GerarNumero()
        {
            return 0;
        }

        // Busca uma conta pelo número.
        public Conta? BuscarNaCollection(int numero)
        {
            return ProcurarPorNumero(numero);
        }

        // Sobrecarga usada internamente para permitir consultas dentro da mesma conexão/transação.
        private Conta? ProcurarPorNumero(int numero, NpgsqlConnection conn, NpgsqlTransaction? transaction, bool bloquearLinha)
        {
            var sql = "SELECT * FROM contas WHERE numero = @numero";
            if (bloquearLinha)
                sql += " FOR UPDATE";

            using var cmd = new NpgsqlCommand(sql, conn);
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.Parameters.AddWithValue("@numero", numero);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapConta(reader);

            return null;
        }

        // Sobrecarga usada por operações que precisam atualizar usando a mesma transação.
        private void Atualizar(Conta conta, NpgsqlConnection conn, NpgsqlTransaction? transaction)
        {
            string sql = "UPDATE contas SET agencia=@agencia, tipo=@tipo, titular=@titular, saldo=@saldo, limite=@limite, aniversario=@aniversario, atualizado_em=CURRENT_TIMESTAMP WHERE numero=@numero";
            using var cmd = new NpgsqlCommand(sql, conn);
            if (transaction != null)
                cmd.Transaction = transaction;

            cmd.Parameters.AddWithValue("@numero", conta.Numero);
            PreencherParametrosConta(cmd, conta);
            cmd.ExecuteNonQuery();
        }

        // Centraliza o preenchimento dos parâmetros usados no INSERT e UPDATE.
        private static void PreencherParametrosConta(NpgsqlCommand cmd, Conta conta)
        {
            cmd.Parameters.AddWithValue("@agencia", conta.Agencia);
            cmd.Parameters.AddWithValue("@tipo", conta.Tipo);
            cmd.Parameters.AddWithValue("@titular", conta.Titular);
            cmd.Parameters.AddWithValue("@saldo", conta.Saldo);
            cmd.Parameters.AddWithValue("@limite", conta is ContaCorrente cc ? cc.Limite : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@aniversario", conta is ContaPoupanca cp ? cp.Aniversario : (object)DBNull.Value);
        }

        // Converte o resultado do banco em um objeto ContaCorrente ou ContaPoupanca.
        private Conta MapConta(NpgsqlDataReader reader)
        {
            int tipo = reader.GetInt32(reader.GetOrdinal("tipo"));
            if (tipo == 1)
            {
                return new ContaCorrente(
                    reader.GetInt32(reader.GetOrdinal("numero")),
                    reader.GetInt32(reader.GetOrdinal("agencia")),
                    reader.GetString(reader.GetOrdinal("titular")),
                    reader.GetFloat(reader.GetOrdinal("saldo")),
                    reader.IsDBNull(reader.GetOrdinal("limite")) ? 0 : reader.GetFloat(reader.GetOrdinal("limite"))
                );
            }

            return new ContaPoupanca(
                reader.GetInt32(reader.GetOrdinal("numero")),
                reader.GetInt32(reader.GetOrdinal("agencia")),
                reader.GetString(reader.GetOrdinal("titular")),
                reader.GetFloat(reader.GetOrdinal("saldo")),
                reader.IsDBNull(reader.GetOrdinal("aniversario")) ? 0 : reader.GetInt32(reader.GetOrdinal("aniversario"))
            );
        }
    }
}
