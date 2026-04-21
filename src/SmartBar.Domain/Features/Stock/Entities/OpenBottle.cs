using SmartBar.Domain.Common;

namespace SmartBar.Domain.Features.Stock.Entities;

public class OpenBottle : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public int InitialVolumeMl { get; private set; }
    public decimal RemainingVolumeMl { get; private set; }
    public bool IsEmpty { get; private set; }

    private OpenBottle() { }

    public static OpenBottle Create(Guid productId, int initialVolumeMl)
    {
        Guard.AgainstNegativeOrZero(initialVolumeMl, nameof(initialVolumeMl));

        return new OpenBottle
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            OpenedAt = DateTime.UtcNow,
            InitialVolumeMl = initialVolumeMl,
            RemainingVolumeMl = initialVolumeMl,
            IsEmpty = false
        };
    }

    public void Consume(decimal volumeMl)
    {
        Guard.AgainstNegativeOrZero(volumeMl, nameof(volumeMl));

        if (IsEmpty)
            throw new InvalidOperationException("This bottle is already empty");

        if (volumeMl > RemainingVolumeMl)
            throw new InvalidOperationException(
                $"Cannot consume {volumeMl} ml — only {RemainingVolumeMl} ml remaining");

        RemainingVolumeMl -= volumeMl;

        if (RemainingVolumeMl <= 0)
            IsEmpty = true;
    }
}
