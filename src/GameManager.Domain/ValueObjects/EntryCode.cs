using System.Text;

namespace GameManager.Domain.ValueObjects;

public class EntryCode
{
    private const string ValidEntryCodeCharacters = "ABCEFHJKMNPQRTWXY0123456789";
    
    public string Value { get; }

    public static EntryCode New(int length)
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

    public static EntryCode Of(string value)
    {
        return new EntryCode(value.ToUpper());
    }

    private EntryCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        if (value.Length > 10)
            throw new ArgumentOutOfRangeException(nameof(value), "Entry code is too long");

        Value = value;
    }

    public static implicit operator EntryCode(string value)
    {
        return new EntryCode(value);
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