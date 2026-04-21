using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Domain.Tests.Features.Catalog.ValueObjects;

public class UnitConversionTests
{
    [Fact]
    public void Constructor_ValidConversion_CreatesUnitConversion()
    {
        var conversion = new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, 20);

        Assert.Equal(UnitOfMeasure.Pack, conversion.FromUnit);
        Assert.Equal(UnitOfMeasure.Piece, conversion.ToUnit);
        Assert.Equal(20m, conversion.Factor);
    }

    [Fact]
    public void Constructor_SameFromAndToUnit_Throws()
    {
        Assert.Throws<InvalidOperationException>(
            () => new UnitConversion(UnitOfMeasure.Piece, UnitOfMeasure.Piece, 1));
    }

    [Fact]
    public void Constructor_ZeroFactor_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, 0));
    }

    [Fact]
    public void Constructor_NegativeFactor_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, -5));
    }
}
