namespace GameManager.Domain.ValueObjects;

public class PlayerName
{
    public string Value { get; }

    public static Result<PlayerName> From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PlayerName>("Name is required");
        if (value.Length < 3)
            return Result.Failure<PlayerName>("Name is too short");
        if (value.Length > 20)
            return Result.Failure<PlayerName>("Name is too long");
        
        return new PlayerName(value);
    }
    
    private PlayerName(string value)
    {
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