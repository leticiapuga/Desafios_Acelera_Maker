using ProjetoContaBancaria.Models;
using System;

namespace ProjetoContaBancaria.Models
{
    // Herda de Conta e adiciona o campo Limite 
    public class ContaCorrente : Conta
    {

        // Limite de crédito disponível
        public float Limite { get; set; }


        // Construtor da Conta Corrente
        // tipo = 1 - Conta corrente
        public ContaCorrente(int numero, int agencia, string titular, float saldo, float limite)
            : base(numero, agencia, 1, titular, saldo)
        {
            Limite = limite;
        }


        // Permite saque até o valor do saldo + limite 
        public override bool Sacar(float valor)
        {
            if (valor > 0 && (Saldo + Limite) >= valor)
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


        // Exibe as informações da conta corrente 
        public void Visualizar()
        {
            Console.WriteLine($"Conta Corrente: {Numero} | Titular: {Titular} | Saldo: {Saldo:C} | Limite: {Limite:C}");
        }
    }
}
