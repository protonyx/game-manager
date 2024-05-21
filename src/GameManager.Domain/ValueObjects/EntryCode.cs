using System.Text;

namespace GameManager.Domain.ValueObjects;

public class EntryCode
{
    private const string ValidEntryCodeCharacters = "ABCEFHJKMNPQRTWXY0123456789";

    private const int DefaultEntryCodeLength = 4;

    public string Value { get; }

    public static EntryCode New(int length = DefaultEntryCodeLength)
    {
        var charSet = new HashSet<char>(ValidEntryCodeCharacters);

        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            var idx = Random.Shared.Next() % charSet.Count;
            var nextChar = charSet.ElementAt(idx);

            sb.Append(nextChar);

            charSet.Remove(nextChar);
        }

        return new EntryCode(sb.ToString());
    }

    public static Result<EntryCode> From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<EntryCode>("Entry code is empty");
        if (value.Length > 10)
            return Result.Failure<EntryCode>("Entry code is too long");

        return new EntryCode(value);
    }

    private EntryCode(string value)
    {
        Value = value.ToUpper();
    }

    public static implicit operator EntryCode(string value)
    {
        return EntryCode.From(value).Value;
    }

    public static implicit operator string(EntryCode code)
    {
        return code.Value;
    }

    private bool Equals(EntryCode other)
        => string.Equals(other.Value, Value, StringComparison.CurrentCultureIgnoreCase);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is EntryCode other && Equals(other);

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}