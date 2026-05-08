using ProjetoContaBancaria.Models;
using Xunit;

namespace ProjetoContaBancaria.Tests;

public class TransferenciaTests
{
    [Fact]
    public void Transferir_EntreContasComSaldoSuficiente_DeveDebitarOrigemECreditarDestino()
    {
        var origem = new ContaPoupanca(1, 1234, "Origem", 300f, 5);
        var destino = new ContaCorrente(2, 1234, "Destino", 100f, 50f);

        var saqueRealizado = origem.Sacar(120f);
        if (saqueRealizado)
            destino.Depositar(120f);

        Assert.True(saqueRealizado);
        Assert.Equal(180f, origem.Saldo);
        Assert.Equal(220f, destino.Saldo);
    }

    [Fact]
    public void Transferir_SemSaldoSuficiente_NaoDeveAlterarNenhumaConta()
    {
        var origem = new ContaPoupanca(1, 1234, "Origem", 100f, 5);
        var destino = new ContaCorrente(2, 1234, "Destino", 50f, 20f);

        var saqueRealizado = origem.Sacar(150f);
        if (saqueRealizado)
            destino.Depositar(150f);

        Assert.False(saqueRealizado);
        Assert.Equal(100f, origem.Saldo);
        Assert.Equal(50f, destino.Saldo);
    }

    [Fact]
    public void Transferir_DeContaCorrenteUsandoLimite_DevePermitirQuandoSaldoMaisLimiteForSuficiente()
    {
        var origem = new ContaCorrente(1, 1234, "Origem", 100f, 200f);
        var destino = new ContaPoupanca(2, 1234, "Destino", 50f, 15);

        var saqueRealizado = origem.Sacar(250f);
        if (saqueRealizado)
            destino.Depositar(250f);

        Assert.True(saqueRealizado);
        Assert.Equal(-150f, origem.Saldo);
        Assert.Equal(300f, destino.Saldo);
    }
}
