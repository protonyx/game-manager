using System.Diagnostics;

namespace GameManager.Domain.ValueObjects;

[DebuggerDisplay("{Value}")]
public class GameName
{
    public const int MinimumLength = 3;

    public const int MaximumLength = 50;

    public string Value { get; }

    public static Result<GameName> From(string value)
    {
        string trimmedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            return Result.Failure<GameName>("Name is required");
        }

        if (trimmedValue.Length < MinimumLength)
        {
            return Result.Failure<GameName>("Name is too short");
        }

        if (trimmedValue.Length > MaximumLength)
        {
            return Result.Failure<GameName>("Name is too long");
        }

        return new GameName(trimmedValue);
    }

    private GameName(string value)
    {
        Value = value;
    }

    private bool Equals(GameName other)
        => string.Equals(other.Value, Value, StringComparison.CurrentCultureIgnoreCase);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is GameName other && Equals(other);

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}