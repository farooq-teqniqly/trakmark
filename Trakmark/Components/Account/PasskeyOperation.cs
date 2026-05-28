namespace Trakmark.Components.Account;

/// <summary>Distinguishes between passkey registration and assertion (sign-in) flows.</summary>
public enum PasskeyOperation
{
    /// <summary>Register a new passkey credential with the authenticator.</summary>
    Create = 0,

    /// <summary>Authenticate using an existing passkey credential.</summary>
    Request = 1,
}
