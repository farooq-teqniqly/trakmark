### Requirement: Auto-register new user from Google email claim
When a new user completes Google OAuth and no local account exists, the system SHALL automatically create a local account using the verified email from the Google claim, create and persist a `RegisteredUser` aggregate for that account, and sign the user in without presenting a registration form.

#### Scenario: New user with email claim auto-registered
- **WHEN** a new user completes Google OAuth
- **AND** the Google principal includes a `ClaimTypes.Email` claim
- **AND** no local account exists for that email
- **THEN** the system SHALL create a local account with that email as username and email address
- **AND** mark the email as confirmed
- **AND** assign the "User" role
- **AND** create and persist a `RegisteredUser` with a new `RegisteredUserId` linked to the new Identity user ID
- **AND** sign the user in
- **AND** redirect to `/`

#### Scenario: New user without email claim redirected to error
- **WHEN** a new user completes Google OAuth
- **AND** the Google principal does NOT include a `ClaimTypes.Email` claim
- **THEN** the system SHALL redirect to `Account/Login` with the status message "Could not retrieve email from Google. Please try again."

#### Scenario: RegisteredUser creation failure redirects to error
- **WHEN** a new user completes Google OAuth
- **AND** the local account is created successfully
- **AND** persisting the `RegisteredUser` fails
- **THEN** the system SHALL redirect to `Account/Login` with an error status message
- **AND** SHALL NOT sign the user in
