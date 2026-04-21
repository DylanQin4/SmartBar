using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Application.Features.Catalog.Products.Commands.CreateProduct;
using SmartBar.Application.Tests.Common.Fixtures;
using SmartBar.Domain.Features.Catalog.Entities;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Application.Tests.Features.Catalog.Commands;

public class CreateProductValidatorTests
{
    private readonly IApplicationDbContext _context;
    private readonly CreateProductCommandValidator _validator;

    public CreateProductValidatorTests()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _validator = new CreateProductCommandValidator(_context);
        SetupEmptyProducts();
    }

    private void SetupEmptyProducts()
    {
        var products = Enumerable.Empty<Product>().AsQueryable();
        var dbSet = Substitute.For<DbSet<Product>, IQueryable<Product>, IAsyncEnumerable<Product>>();

        ((IQueryable<Product>)dbSet).Provider.Returns(new TestAsyncQueryProvider<Product>(products.Provider));
        ((IQueryable<Product>)dbSet).Expression.Returns(products.Expression);
        ((IQueryable<Product>)dbSet).ElementType.Returns(products.ElementType);
        ((IQueryable<Product>)dbSet).GetEnumerator().Returns(products.GetEnumerator());

        _context.Products.Returns(dbSet);
    }

    private static CreateProductCommand ValidBeverageCommand() => new()
    {
        Name = "Coca-Cola",
        Category = ProductCategory.Beverage,
        IsSellable = true,
        IsIngredient = false,
        SellingPrice = 3000,
        BaseUnit = UnitOfMeasure.Piece,
        LowStockThreshold = 5
    };

    [Fact]
    public async Task Validate_ValidBeverage_Succeeds()
    {
        var result = await _validator.ValidateAsync(ValidBeverageCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyName_Fails()
    {
        var command = ValidBeverageCommand() with { Name = "" };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_SellableWithZeroPrice_Fails()
    {
        var command = ValidBeverageCommand() with { SellingPrice = 0 };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_AlcoholWithoutVolume_Fails()
    {
        var command = ValidBeverageCommand() with
        {
            Category = ProductCategory.Alcohol,
            VolumeInMl = null
        };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_NeitherSellableNorIngredient_Fails()
    {
        var command = ValidBeverageCommand() with { IsSellable = false, IsIngredient = false };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_NegativeStockThreshold_Fails()
    {
        var command = ValidBeverageCommand() with { LowStockThreshold = -1 };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_NightPriceZero_Fails()
    {
        var command = ValidBeverageCommand() with { NightPrice = 0 };

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }
}
