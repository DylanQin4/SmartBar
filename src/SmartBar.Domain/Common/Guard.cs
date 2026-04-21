namespace SmartBar.Domain.Common;

public static class Guard
{
    public static void AgainstNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
    }

    public static void AgainstNegative(decimal value, string paramName)
    {
        if (value < 0)
            throw new ArgumentException($"{paramName} cannot be negative.", paramName);
    }

    public static void AgainstNegativeOrZero(decimal value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be greater than zero.", paramName);
    }

    public static void AgainstNull<T>(T? value, string paramName) where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
    }

    public static void AgainstInvalidEnum<T>(T value, string paramName) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            throw new ArgumentException($"{paramName} has an invalid value: {value}.", paramName);
    }
}
