namespace Trakmark.Domain.Ids;

/// <summary>
/// Bridges the domain to ASP.NET Core Identity by wrapping the external account key
/// as-is. Not subject to the domain identifier format (no prefix, no Crockford constraint).
/// </summary>
public readonly record struct UserAccountId
{
    /// <summary>The external ASP.NET Identity account key, unchanged.</summary>
    public string Value { get; }

    /// <summary>Initializes a <see cref="UserAccountId"/> from an external account key.</summary>
    public UserAccountId(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
