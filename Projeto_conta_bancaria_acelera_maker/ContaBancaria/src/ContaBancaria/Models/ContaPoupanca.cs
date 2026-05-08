using ProjetoContaBancaria.Models;
using System;

namespace ProjetoContaBancaria.Models
{
    // Herda de Conta e adiciona o campo Aniversario (dia do mês para rendimento)
    public class ContaPoupanca : Conta
    {

        // Dia do mês em que a conta faz aniversário (usado para cálculo de rendimento)
        public int Aniversario { get; set; }


        // Construtor da Conta Poupança
        // tipo = 2 - Conta poupança
        public ContaPoupanca(int numero, int agencia, string titular, float saldo, int aniversario)
            : base(numero, agencia, 2, titular, saldo)
        {
            Aniversario = aniversario;
        }


        // Realiza o saque se houver saldo suficiente
        public override bool Sacar(float valor)
        {
            if (valor > 0 && Saldo >= valor)
            {
                Saldo -= valor;
                return true;
            }
            return false;
        }


        // Realiza o depósito se o valor for positivo
        public override void Depositar(float valor)
        {
            if (valor > 0)
                Saldo += valor;
        }


        // Exibe as informações da conta poupança de forma amigável
        public void Visualizar()
        {
            Console.WriteLine($"Conta Poupança: {Numero} | Titular: {Titular} | Saldo: {Saldo:C} | Aniversário: {Aniversario}");
        }
    }
}
