using System.Diagnostics;

namespace GameManager.Domain.ValueObjects;

[DebuggerDisplay("{Value}")]
public class PlayerName
{
    public const int MinimumLength = 3;

    public const int MaximumLength = 20;

    public string Value { get; }

    public static Result<PlayerName> From(string value)
    {
        string trimmedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            return Result.Failure<PlayerName>("Name is required");
        }

        if (trimmedValue.Length < MinimumLength)
        {
            return Result.Failure<PlayerName>("Name is too short");
        }

        if (trimmedValue.Length > MaximumLength)
        {
            return Result.Failure<PlayerName>("Name is too long");
        }

        return new PlayerName(trimmedValue);
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

    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
}