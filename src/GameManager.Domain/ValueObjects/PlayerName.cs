namespace GameManager.Domain.ValueObjects;

public class PlayerName
{
    public string Value { get; }

    public static PlayerName Of(string value)
    {
        return new PlayerName(value);
    }
    
    private PlayerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        if (value.Length > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Name is too long");

        Value = value;
    }
    
    private bool Equals(PlayerName other)
        => string.Equals(other.Value, Value, StringComparison.CurrentCultureIgnoreCase);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is PlayerName other && Equals(other);
    
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}