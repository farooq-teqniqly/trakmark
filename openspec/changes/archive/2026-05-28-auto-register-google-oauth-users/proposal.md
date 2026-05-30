## Why

New users who sign in via Google are shown a redundant email entry form — Google already provides a verified email claim. The extra step creates unnecessary friction and implies the user needs to do work they don't need to do.

## What Changes

- Remove the email input form from `ExternalLogin.razor`
- Auto-create new user accounts using the email from the Google claim
- Redirect all users to `/` after successful auth (removing `ReturnUrl` dependency)
- If Google provides no email claim, redirect to login with a descriptive error
- Remove all unused Identity scaffold: 2FA pages, passkey pages/endpoints/models, email-change page, external login management page, no-op email sender, and associated log methods

## Capabilities

### New Capabilities

- `google-oauth-auto-registration`: Auto-register new Google OAuth users without manual email entry; redirect to homepage on success

### Modified Capabilities

## Impact

- `Trakmark/Components/Account/Pages/ExternalLogin.razor` — core logic change
- 18 razor files deleted (2FA, passkey, email-change, external login management pages)
- 5 C# files deleted (`PasskeyInputModel`, `PasskeyOperation`, `IdentityNoOpEmailSender`, orphaned log methods)
- 3 API endpoints removed (`/PasskeyCreationOptions`, `/PasskeyRequestOptions`, `/Manage/LinkExternalLogin`)
- `ServiceCollectionExtensions.cs` — removed `IEmailSender` registration
- `ManageNavMenu.razor`, `App.razor` — trimmed to match supported features
- No database schema changes
- No new dependencies
