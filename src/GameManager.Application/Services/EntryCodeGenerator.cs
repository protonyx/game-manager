using System.Text;

namespace GameManager.Application.Services;

public static class EntryCodeGenerator
{
    private const string ValidEntryCodeCharacters = "ABCEFHJKMNPQRTWXY0123456789";

    public static string Create(int length)
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

        return sb.ToString();
    }
}