using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Domain.Tests.Features.Catalog.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ValidAmount_CreatesMoney()
    {
        var money = new Money(1000);

        Assert.Equal(1000m, money.Amount);
        Assert.Equal("MGA", money.Currency);
    }

    [Fact]
    public void Constructor_NegativeAmount_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Money(-1));
    }

    [Fact]
    public void Zero_ReturnsZeroMGA()
    {
        var money = Money.Zero();

        Assert.Equal(0m, money.Amount);
        Assert.Equal("MGA", money.Currency);
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        var a = new Money(1000);
        var b = new Money(2000);

        var result = a + b;

        Assert.Equal(3000m, result.Amount);
    }

    [Fact]
    public void Add_DifferentCurrency_Throws()
    {
        var a = new Money(1000, "MGA");
        var b = new Money(2000, "EUR");

        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void Subtract_SameCurrency_ReturnsDifference()
    {
        var a = new Money(3000);
        var b = new Money(1000);

        var result = a - b;

        Assert.Equal(2000m, result.Amount);
    }

    [Fact]
    public void Multiply_ByFactor_ReturnsProduct()
    {
        var money = new Money(500);

        var result = money * 3;

        Assert.Equal(1500m, result.Amount);
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_AreEqual()
    {
        var a = new Money(1000, "MGA");
        var b = new Money(1000, "MGA");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentAmount_AreNotEqual()
    {
        var a = new Money(1000);
        var b = new Money(2000);

        Assert.NotEqual(a, b);
    }
}
