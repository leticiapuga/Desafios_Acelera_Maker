using ProjetoContaBancaria.Models;
using System.Collections.Generic;

namespace ProjetoContaBancaria.Repositories
{
    public interface IContaRepository
    {
        Conta? ProcurarPorNumero(int numero);
        List<Conta> ListarTodas();
        int Cadastrar(Conta conta);
        void Atualizar(Conta conta);
        void Deletar(int numero);
        void Sacar(int numero, float valor);
        void Depositar(int numero, float valor);
        void Transferir(int numeroOrigem, int numeroDestino, float valor);
    }
}
