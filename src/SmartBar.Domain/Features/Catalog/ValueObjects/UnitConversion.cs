using SmartBar.Domain.Common;
using SmartBar.Domain.Features.Catalog.Enums;

namespace SmartBar.Domain.Features.Catalog.ValueObjects;

public class UnitConversion : ValueObject
{
    public Guid Id { get; }
    public UnitOfMeasure FromUnit { get; }
    public UnitOfMeasure ToUnit { get; }
    public decimal Factor { get; }

    public UnitConversion(Guid id, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, decimal factor)
    {
        if (fromUnit == toUnit)
            throw new InvalidOperationException("FromUnit and ToUnit cannot be the same");

        Guard.AgainstNegativeOrZero(factor, nameof(factor));

        Id = id;
        FromUnit = fromUnit;
        ToUnit = toUnit;
        Factor = factor;
    }

    public UnitConversion(UnitOfMeasure fromUnit, UnitOfMeasure toUnit, decimal factor)
        : this(Guid.NewGuid(), fromUnit, toUnit, factor) { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FromUnit;
        yield return ToUnit;
        yield return Factor;
    }
}
