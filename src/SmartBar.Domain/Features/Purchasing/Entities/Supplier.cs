using SmartBar.Domain.Common;

namespace SmartBar.Domain.Features.Purchasing.Entities;

public class Supplier : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }

    private Supplier() { }

    public static Supplier Create(string name, string? phone)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));

        if (name.Length > 200)
            throw new InvalidOperationException("Supplier name cannot exceed 200 characters");

        return new Supplier
        {
            Id = Guid.NewGuid(),
            Name = name,
            Phone = phone,
            IsActive = true
        };
    }

    public void Update(string name, string? phone)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));

        if (name.Length > 200)
            throw new InvalidOperationException("Supplier name cannot exceed 200 characters");

        Name = name;
        Phone = phone;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
