using ProjetoContaBancaria.Models;
using Xunit;

namespace ProjetoContaBancaria.Tests;

public class ContaCorrenteTests
{
    [Fact]
    public void Depositar_ValorPositivo_DeveAumentarSaldo()
    {
        var conta = new ContaCorrente(1, 1234, "Maria", 100f, 200f);

        conta.Depositar(50f);

        Assert.Equal(150f, conta.Saldo);
    }

    [Fact]
    public void Depositar_ValorNegativo_NaoDeveAlterarSaldo()
    {
        var conta = new ContaCorrente(1, 1234, "Maria", 100f, 200f);

        conta.Depositar(-50f);

        Assert.Equal(100f, conta.Saldo);
    }

    [Fact]
    public void Sacar_UsandoSaldoDisponivel_DeveRetornarVerdadeiroEAtualizarSaldo()
    {
        var conta = new ContaCorrente(1, 1234, "Maria", 100f, 200f);

        var resultado = conta.Sacar(250f);

        Assert.True(resultado);
        Assert.Equal(-150f, conta.Saldo);
    }

    [Fact]
    public void Sacar_AcimaDoSaldoMaisLimite_DeveRetornarFalsoENaoAlterarSaldo()
    {
        var conta = new ContaCorrente(1, 1234, "Maria", 100f, 200f);

        var resultado = conta.Sacar(350f);

        Assert.False(resultado);
        Assert.Equal(100f, conta.Saldo);
    }

    [Fact]
    public void Sacar_ValorZero_DeveRetornarFalsoENaoAlterarSaldo()
    {
        var conta = new ContaCorrente(1, 1234, "Maria", 100f, 200f);

        var resultado = conta.Sacar(0f);

        Assert.False(resultado);
        Assert.Equal(100f, conta.Saldo);
    }
}
