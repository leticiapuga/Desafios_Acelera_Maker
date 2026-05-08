using System;

namespace ProjetoContaBancaria.Models
{
    // Classe base abstrata para contas bancárias
    public abstract class Conta
    {
        // Número único da conta
        public int Numero { get; set; }

        // Número da agência (até 6 dígitos, validado na criação)
        public int Agencia { get; set; }

        // Tipo da conta: 1 = Corrente, 2 = Poupança
        public int Tipo { get; set; }

        // Nome do titular da conta
        public string Titular { get; set; }

        // Saldo atual da conta (só pode ser alterado pelas operações)
        public float Saldo { get; protected set; }

        // Construtor base para inicializar os dados essenciais da conta
        public Conta(int numero, int agencia, int tipo, string titular, float saldo)
        {
            Numero = numero;
            Agencia = agencia;
            Tipo = tipo;
            Titular = titular;
            Saldo = saldo;
        }

        // Método abstrato para saque: cada tipo de conta implementa sua própria regra
        public abstract bool Sacar(float valor);

        // Método abstrato para depósito: cada tipo de conta implementa sua própria regra
        public abstract void Depositar(float valor);
    }
}
