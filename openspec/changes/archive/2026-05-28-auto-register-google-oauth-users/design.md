## Context

`ExternalLogin.razor` is the callback page for OAuth providers. The ASP.NET Identity scaffold generates it with an email confirmation form — intended for providers that don't supply a verified email. Google always provides a verified `ClaimTypes.Email`. The form is redundant and adds friction.

## Goals / Non-Goals

**Goals:**
- Skip email form entirely for Google OAuth
- Auto-create new user account from Google email claim
- Redirect to `/` after all successful auth paths

**Non-Goals:**
- Supporting other OAuth providers (only Google is configured)
- Preserving `ReturnUrl` behavior
- Email confirmation flow (Google email is already verified)

## Decisions

**Auto-create on callback, not on form submit**
`OnLoginCallbackAsync` already reads the email claim. When the user has no existing account and an email claim is present, create the account immediately instead of rendering a form. Eliminates the `InputModel`, form, and `OnValidSubmitAsync` entirely.

**Always redirect to `/`**
No existing users, no bookmarked protected pages. `ReturnUrl` has no value right now. Both the existing-user and new-user paths redirect to `"/"`.

**No email claim → redirect to login with error**
If Google omits the email claim (rare but possible), redirect to `Account/Login` with a status message. Silent failure or blank account creation are worse outcomes.

## Risks / Trade-offs

- [Future OAuth providers may not supply email] → If a second provider is added without an email claim, this code path will error. Mitigation: the no-email-claim guard handles it gracefully.
- [ReturnUrl dropped permanently] → If protected routes are added later, users won't be deep-linked after login. Mitigation: restore `ReturnUrl` redirect when needed.

## Migration Plan

No data migration. Deploy is safe to roll forward or back — no schema changes.

Files changed: `ExternalLogin.razor` (logic rewrite), plus removal of 18 razor pages, 5 C# files, 3 API endpoints, and supporting registrations for unused Identity scaffold (2FA, passkeys, email-change, external login management).
