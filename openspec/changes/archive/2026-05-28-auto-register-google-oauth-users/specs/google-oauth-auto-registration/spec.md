## ADDED Requirements

### Requirement: Auto-register new user from Google email claim
When a new user completes Google OAuth and no local account exists, the system SHALL automatically create a local account using the verified email from the Google claim and sign the user in without presenting a registration form.

#### Scenario: New user with email claim auto-registered
- **WHEN** a new user completes Google OAuth
- **AND** the Google principal includes a `ClaimTypes.Email` claim
- **AND** no local account exists for that email
- **THEN** the system SHALL create a local account with that email as username and email address
- **AND** mark the email as confirmed
- **AND** assign the "User" role
- **AND** sign the user in
- **AND** redirect to `/`

#### Scenario: New user without email claim redirected to error
- **WHEN** a new user completes Google OAuth
- **AND** the Google principal does NOT include a `ClaimTypes.Email` claim
- **THEN** the system SHALL redirect to `Account/Login` with the status message "Could not retrieve email from Google. Please try again."

### Requirement: Redirect all successful auth to homepage
After any successful authentication via Google OAuth, the system SHALL redirect the user to `/` regardless of any `ReturnUrl` parameter.

#### Scenario: Existing user redirected to homepage
- **WHEN** an existing user completes Google OAuth sign-in
- **THEN** the system SHALL redirect to `/`

#### Scenario: New user redirected to homepage after auto-registration
- **WHEN** a new user is auto-registered via Google OAuth
- **THEN** the system SHALL redirect to `/`

### Requirement: Remove unused auth scaffolding
The system SHALL NOT expose pages or endpoints for authentication features that are not supported: local passwords, 2FA, TOTP authenticators, recovery codes, passkeys, or external login management.

#### Scenario: Removed pages return 404
- **WHEN** a user navigates to any removed account management page (e.g. `/Account/Manage/TwoFactorAuthentication`, `/Account/Manage/Passkeys`, `/Account/Manage/Email`, `/Account/Manage/ExternalLogins`)
- **THEN** the system SHALL return a 404 response

#### Scenario: Account management nav shows only supported options
- **WHEN** an authenticated user views the account management navigation
- **THEN** the nav SHALL contain only "Profile" and "Personal data" links
