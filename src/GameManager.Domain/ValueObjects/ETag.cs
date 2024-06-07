namespace GameManager.Domain.ValueObjects;

public class ETag
{
    public string Value { get; }

    public bool IsWeak { get; }

    private ETag(string value, bool isWeak = false)
    {
        Value = value;
        IsWeak = isWeak;
    }

    public override string ToString()
    {
        return IsWeak ? $"W/\"{Value}\"" : $"\"{Value}\"";
    }

    public static ETag Empty()
    {
        return new ETag(string.Empty);
    }

    public static ETag From(object value)
    {
        var temp = value?.ToString();
        
        if (string.IsNullOrWhiteSpace(temp))
        {
            return Empty();
        }

        temp = temp.Trim();
        
        if (temp.StartsWith("W/"))
        {
            return new ETag(temp.Substring(3, temp.Length - 4), true);
        }
        else if (temp.StartsWith("\""))
        {
            return new ETag(temp.Substring(1, temp.Length - 2));
        }
        
        return new ETag(temp);
    }

    public static ETag FromWeak(object value)
    {
        return new ETag(value.ToString() ?? string.Empty, true);
    }

    public static implicit operator string(ETag etag)
    {
        return etag.ToString();
    }
    
    private bool Equals(ETag other)
        => string.Equals(other.ToString(), this.ToString(), StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is ETag other && Equals(other);

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}