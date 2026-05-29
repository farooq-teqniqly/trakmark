## Why

New users who sign in via Google are shown a redundant email entry form — Google already provides a verified email claim. The extra step creates unnecessary friction and implies the user needs to do work they don't need to do.

## What Changes

- Remove the email input form from `ExternalLogin.razor`
- Auto-create new user accounts using the email from the Google claim
- Redirect all users to `/` after successful auth (removing `ReturnUrl` dependency)
- If Google provides no email claim, redirect to login with a descriptive error

## Capabilities

### New Capabilities

- `google-oauth-auto-registration`: Auto-register new Google OAuth users without manual email entry; redirect to homepage on success

### Modified Capabilities

## Impact

- `Trakmark/Components/Account/Pages/ExternalLogin.razor` — only file changed
- No database schema changes
- No API changes
- No new dependencies
