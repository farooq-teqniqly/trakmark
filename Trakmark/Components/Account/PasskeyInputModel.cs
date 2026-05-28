namespace Trakmark.Components.Account;

/// <summary>Holds the raw credential JSON and any error message from a passkey operation.</summary>
public class PasskeyInputModel
{
    /// <summary>Gets or sets the JSON-serialized credential returned by the browser's WebAuthn API.</summary>
    public string? CredentialJson { get; set; }

    /// <summary>Gets or sets an error message if the passkey operation failed.</summary>
    public string? Error { get; set; }
}
