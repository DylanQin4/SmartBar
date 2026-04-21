using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.Enums;
using SmartBar.Domain.Features.Catalog.ValueObjects;

namespace SmartBar.Domain.Features.Catalog.Entities;

public class Product : AggregateRoot
{
    private readonly List<UnitConversion> _unitConversions = new();

    public string Name { get; private set; } = null!;
    public ProductCategory Category { get; private set; }
    public bool IsSellable { get; private set; }
    public bool IsIngredient { get; private set; }
    public Money SellingPrice { get; private set; } = null!;
    public Money? NightPrice { get; private set; }
    public int? VolumeInMl { get; private set; }
    public UnitOfMeasure BaseUnit { get; private set; }
    public int LowStockThreshold { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyList<UnitConversion> UnitConversions => _unitConversions.AsReadOnly();

    private Product() { }

    public static Product Create(
        string name,
        ProductCategory category,
        bool isSellable,
        bool isIngredient,
        Money sellingPrice,
        Money? nightPrice,
        int? volumeInMl,
        UnitOfMeasure baseUnit,
        int lowStockThreshold)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));

        if (name.Length > 200)
            throw new InvalidOperationException("Product name cannot exceed 200 characters");

        if (!isSellable && !isIngredient)
            throw new InvalidOperationException("A product must be sellable or an ingredient (or both)");

        if (isSellable)
            Guard.AgainstNegativeOrZero(sellingPrice.Amount, nameof(sellingPrice));

        if (category == ProductCategory.Alcohol && (!volumeInMl.HasValue || volumeInMl.Value <= 0))
            throw new InvalidOperationException("Volume is required for alcohol products");

        if (nightPrice is not null)
            Guard.AgainstNegativeOrZero(nightPrice.Amount, nameof(nightPrice));

        if (lowStockThreshold < 0)
            throw new InvalidOperationException("Low stock threshold cannot be negative");

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            IsSellable = isSellable,
            IsIngredient = isIngredient,
            SellingPrice = sellingPrice,
            NightPrice = nightPrice,
            VolumeInMl = volumeInMl,
            BaseUnit = baseUnit,
            LowStockThreshold = lowStockThreshold,
            IsActive = true
        };
    }

    public void Update(
        string name,
        ProductCategory category,
        bool isSellable,
        bool isIngredient,
        Money sellingPrice,
        Money? nightPrice,
        int? volumeInMl,
        UnitOfMeasure baseUnit,
        int lowStockThreshold)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));

        if (name.Length > 200)
            throw new InvalidOperationException("Product name cannot exceed 200 characters");

        if (!isSellable && !isIngredient)
            throw new InvalidOperationException("A product must be sellable or an ingredient (or both)");

        if (isSellable)
            Guard.AgainstNegativeOrZero(sellingPrice.Amount, nameof(sellingPrice));

        if (category == ProductCategory.Alcohol && (!volumeInMl.HasValue || volumeInMl.Value <= 0))
            throw new InvalidOperationException("Volume is required for alcohol products");

        if (nightPrice is not null)
            Guard.AgainstNegativeOrZero(nightPrice.Amount, nameof(nightPrice));

        if (lowStockThreshold < 0)
            throw new InvalidOperationException("Low stock threshold cannot be negative");

        Name = name;
        Category = category;
        IsSellable = isSellable;
        IsIngredient = isIngredient;
        SellingPrice = sellingPrice;
        NightPrice = nightPrice;
        VolumeInMl = volumeInMl;
        BaseUnit = baseUnit;
        LowStockThreshold = lowStockThreshold;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public Money GetPriceAt(TimeOnly time, TimeOnly nightStartTime)
    {
        if (!IsSellable)
            throw new InvalidOperationException("This product is not sellable");

        if (NightPrice is not null && time >= nightStartTime)
            return NightPrice;

        return SellingPrice;
    }

    public void SetUnitConversions(List<UnitConversion> conversions)
    {
        var duplicates = conversions
            .GroupBy(c => new { c.FromUnit, c.ToUnit })
            .Any(g => g.Count() > 1);

        if (duplicates)
            throw new InvalidOperationException("Duplicate unit conversion detected (same FromUnit + ToUnit)");

        _unitConversions.Clear();
        _unitConversions.AddRange(conversions);
    }

    public decimal ConvertToBaseUnit(decimal quantity, UnitOfMeasure fromUnit)
    {
        if (fromUnit == BaseUnit)
            return quantity;

        var conversion = _unitConversions
            .FirstOrDefault(c => c.FromUnit == fromUnit && c.ToUnit == BaseUnit);

        if (conversion is null)
            throw new InvalidOperationException(
                $"No conversion found from {fromUnit} to {BaseUnit} for product {Name}");

        return quantity * conversion.Factor;
    }
}
