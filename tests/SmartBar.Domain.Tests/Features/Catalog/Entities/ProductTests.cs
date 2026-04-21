using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Domain.Tests.Features.Catalog.Entities;

public class ProductTests
{
    private static Product CreateBeverage(decimal price = 3000) =>
        Product.Create("Coca-Cola", ProductCategory.Beverage,
            isSellable: true, isIngredient: false,
            new Money(price), nightPrice: null, volumeInMl: null,
            UnitOfMeasure.Piece, lowStockThreshold: 5);

    private static Product CreateAlcohol(decimal price = 20000, int volumeMl = 1000) =>
        Product.Create("Rhum", ProductCategory.Alcohol,
            isSellable: true, isIngredient: false,
            new Money(price), nightPrice: new Money(25000), volumeInMl: volumeMl,
            UnitOfMeasure.Bottle, lowStockThreshold: 3);

    // --- Create validation ---

    [Fact]
    public void Create_ValidBeverage_Succeeds()
    {
        var product = CreateBeverage();

        Assert.Equal("Coca-Cola", product.Name);
        Assert.Equal(ProductCategory.Beverage, product.Category);
        Assert.True(product.IsActive);
        Assert.True(product.IsSellable);
    }

    [Fact]
    public void Create_EmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Product.Create("", ProductCategory.Beverage, true, false,
                new Money(1000), null, null, UnitOfMeasure.Piece, 0));
    }

    [Fact]
    public void Create_NameTooLong_Throws()
    {
        var longName = new string('x', 201);

        Assert.Throws<InvalidOperationException>(() =>
            Product.Create(longName, ProductCategory.Beverage, true, false,
                new Money(1000), null, null, UnitOfMeasure.Piece, 0));
    }

    [Fact]
    public void Create_NotSellableNorIngredient_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Product.Create("Test", ProductCategory.Beverage, false, false,
                new Money(0), null, null, UnitOfMeasure.Piece, 0));
    }

    [Fact]
    public void Create_AlcoholWithoutVolume_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Product.Create("Rhum", ProductCategory.Alcohol, true, false,
                new Money(20000), null, volumeInMl: null, UnitOfMeasure.Bottle, 0));
    }

    [Fact]
    public void Create_SellableWithZeroPrice_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Product.Create("Test", ProductCategory.Beverage, true, false,
                Money.Zero(), null, null, UnitOfMeasure.Piece, 0));
    }

    [Fact]
    public void Create_IngredientOnlyWithZeroPrice_Succeeds()
    {
        var product = Product.Create("Sucre", ProductCategory.Grocery,
            isSellable: false, isIngredient: true,
            Money.Zero(), null, null, UnitOfMeasure.Piece, 0);

        Assert.False(product.IsSellable);
        Assert.True(product.IsIngredient);
    }

    // --- GetPriceAt ---

    [Fact]
    public void GetPriceAt_BeforeNightTime_ReturnsSellingPrice()
    {
        var product = CreateAlcohol(20000);
        var nightStart = new TimeOnly(21, 0);

        var price = product.GetPriceAt(new TimeOnly(20, 0), nightStart);

        Assert.Equal(20000m, price.Amount);
    }

    [Fact]
    public void GetPriceAt_AfterNightTime_ReturnsNightPrice()
    {
        var product = CreateAlcohol(20000);
        var nightStart = new TimeOnly(21, 0);

        var price = product.GetPriceAt(new TimeOnly(22, 0), nightStart);

        Assert.Equal(25000m, price.Amount);
    }

    [Fact]
    public void GetPriceAt_NoNightPrice_AlwaysReturnsSellingPrice()
    {
        var product = CreateBeverage(3000);
        var nightStart = new TimeOnly(21, 0);

        var price = product.GetPriceAt(new TimeOnly(23, 0), nightStart);

        Assert.Equal(3000m, price.Amount);
    }

    [Fact]
    public void GetPriceAt_NotSellable_Throws()
    {
        var product = Product.Create("Sucre", ProductCategory.Grocery,
            false, true, Money.Zero(), null, null, UnitOfMeasure.Piece, 0);

        Assert.Throws<InvalidOperationException>(
            () => product.GetPriceAt(new TimeOnly(12, 0), new TimeOnly(21, 0)));
    }

    // --- ConvertToBaseUnit ---

    [Fact]
    public void ConvertToBaseUnit_SameUnit_ReturnsOriginalQuantity()
    {
        var product = CreateBeverage();

        var result = product.ConvertToBaseUnit(10, UnitOfMeasure.Piece);

        Assert.Equal(10m, result);
    }

    [Fact]
    public void ConvertToBaseUnit_WithConversion_AppliesFactor()
    {
        var product = Product.Create("Marlboro", ProductCategory.Cigarette,
            true, false, new Money(5000), null, null, UnitOfMeasure.Piece, 10);

        product.SetUnitConversions([
            new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, 20)
        ]);

        var result = product.ConvertToBaseUnit(2, UnitOfMeasure.Pack);

        Assert.Equal(40m, result); // 2 packs × 20 = 40 pieces
    }

    [Fact]
    public void ConvertToBaseUnit_NoConversionExists_Throws()
    {
        var product = CreateBeverage();

        Assert.Throws<InvalidOperationException>(
            () => product.ConvertToBaseUnit(1, UnitOfMeasure.Crate));
    }

    // --- SetUnitConversions ---

    [Fact]
    public void SetUnitConversions_DuplicateFromToUnit_Throws()
    {
        var product = CreateBeverage();

        Assert.Throws<InvalidOperationException>(() =>
            product.SetUnitConversions([
                new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, 20),
                new UnitConversion(UnitOfMeasure.Pack, UnitOfMeasure.Piece, 10)
            ]));
    }

    // --- Activate / Deactivate ---

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var product = CreateBeverage();

        product.Deactivate();

        Assert.False(product.IsActive);
    }

    [Fact]
    public void Activate_AfterDeactivate_SetsIsActiveTrue()
    {
        var product = CreateBeverage();
        product.Deactivate();

        product.Activate();

        Assert.True(product.IsActive);
    }
}
