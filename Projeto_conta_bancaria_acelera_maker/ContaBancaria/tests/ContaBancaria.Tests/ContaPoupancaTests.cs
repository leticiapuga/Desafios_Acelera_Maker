using ProjetoContaBancaria.Models;
using Xunit;

namespace ProjetoContaBancaria.Tests;

public class ContaPoupancaTests
{
    [Fact]
    public void Depositar_ValorPositivo_DeveAumentarSaldo()
    {
        var conta = new ContaPoupanca(2, 1234, "João", 100f, 10);

        conta.Depositar(80f);

        Assert.Equal(180f, conta.Saldo);
    }

    [Fact]
    public void Depositar_ValorNegativo_NaoDeveAlterarSaldo()
    {
        var conta = new ContaPoupanca(2, 1234, "João", 100f, 10);

        conta.Depositar(-80f);

        Assert.Equal(100f, conta.Saldo);
    }

    [Fact]
    public void Sacar_ComSaldoSuficiente_DeveRetornarVerdadeiroEAtualizarSaldo()
    {
        var conta = new ContaPoupanca(2, 1234, "João", 100f, 10);

        var resultado = conta.Sacar(60f);

        Assert.True(resultado);
        Assert.Equal(40f, conta.Saldo);
    }

    [Fact]
    public void Sacar_AcimaDoSaldo_DeveRetornarFalsoENaoAlterarSaldo()
    {
        var conta = new ContaPoupanca(2, 1234, "João", 100f, 10);

        var resultado = conta.Sacar(120f);

        Assert.False(resultado);
        Assert.Equal(100f, conta.Saldo);
    }

    [Fact]
    public void Sacar_ValorZero_DeveRetornarFalsoENaoAlterarSaldo()
    {
        var conta = new ContaPoupanca(2, 1234, "João", 100f, 10);

        var resultado = conta.Sacar(0f);

        Assert.False(resultado);
        Assert.Equal(100f, conta.Saldo);
    }
}
