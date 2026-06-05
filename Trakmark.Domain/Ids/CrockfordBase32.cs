namespace Trakmark.Domain.Ids;

/// <summary>
/// Generates and validates Crockford base32 character sequences.
/// Alphabet: uppercase A–Z and 2–9, excluding the ambiguous characters 0, O, 1, I, L.
/// </summary>
internal static class CrockfordBase32
{
    /// <summary>The valid Crockford base32 alphabet used by domain IDs.</summary>
    internal const string Alphabet = "ABCDEFGHJKMNPQRSTVWXYZ23456789";

    private static readonly Random Random = new();

    /// <summary>Generates a random body of <paramref name="length"/> Crockford base32 characters.</summary>
    internal static string GenerateBody(int length)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = Alphabet[Random.Next(Alphabet.Length)];
        return new string(chars);
    }

    /// <summary>Returns <see langword="true"/> if every character in <paramref name="body"/> is a valid Crockford base32 character.</summary>
    internal static bool IsValidBody(ReadOnlySpan<char> body)
    {
        foreach (var c in body)
        {
            if (Alphabet.IndexOf(c) < 0)
                return false;
        }
        return true;
    }
}
