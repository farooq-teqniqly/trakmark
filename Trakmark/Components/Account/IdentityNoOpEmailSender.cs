using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Trakmark.Data;

namespace Trakmark.Components.Account;

/// <summary>
/// No-op <see cref="IEmailSender{TUser}"/> used during development.
/// Replace with a real implementation before deploying to production, then remove the
/// <c>IdentityNoOpEmailSender</c> branch from <c>RegisterConfirmation.razor</c>.
/// </summary>
internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailSender emailSender = new NoOpEmailSender();

    /// <summary>Sends an account confirmation link to <paramref name="email"/>.</summary>
    public Task SendConfirmationLinkAsync(
        ApplicationUser user,
        string email,
        string confirmationLink
    ) =>
        emailSender.SendEmailAsync(
            email,
            "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>."
        );

    /// <summary>Sends a password-reset code to <paramref name="email"/>.</summary>
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        emailSender.SendEmailAsync(
            email,
            "Reset your password",
            $"Please reset your password using the following code: {resetCode}"
        );

    /// <summary>Sends a password-reset link to <paramref name="email"/>.</summary>
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        emailSender.SendEmailAsync(
            email,
            "Reset your password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>."
        );
}
