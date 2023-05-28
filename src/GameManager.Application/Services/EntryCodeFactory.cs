using System.Text;

namespace GameManager.Server.Services;

public static class EntryCodeFactory
{
    private const string ValidEntryCodeCharacters = "ABCEFHJKMNPQRTWXY0123456789";

    public static string Create(int length)
    {
        var sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            var idx = Random.Shared.Next() % ValidEntryCodeCharacters.Length;

            sb.Append(ValidEntryCodeCharacters[idx]);
        }

        return sb.ToString();
    }
}