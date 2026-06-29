### Requirement: Persist RegisteredUser on first Google OAuth login
When a new user completes Google OAuth registration, the system SHALL create a `RegisteredUser` aggregate via `RegisteredUser.Create(UserAccountId)` and persist it to the `RegisteredUsers` table before signing the user in.

#### Scenario: RegisteredUser persisted on new user registration
- **WHEN** a new user completes Google OAuth and a local account is created
- **THEN** the system SHALL persist a `RegisteredUser` row to the `RegisteredUsers` table with the new `RegisteredUserId` and the Identity user's GUID as `AccountId`

#### Scenario: Registration fails if RegisteredUser cannot be persisted
- **WHEN** a new user completes Google OAuth
- **AND** persisting the `RegisteredUser` fails (e.g., database error)
- **THEN** the system SHALL redirect to `Account/Login` with an error status message
- **AND** SHALL NOT sign the user in

### Requirement: Resolve RegisteredUserId from Identity user ID
The system SHALL provide `IRegisteredUserLookupService` with a method that accepts an ASP.NET Identity user ID string and returns the corresponding `RegisteredUserId` by querying the `RegisteredUsers` table.

#### Scenario: Lookup returns RegisteredUserId for known user
- **WHEN** `GetByAccountIdAsync` is called with an Identity user ID that has a persisted `RegisteredUser`
- **THEN** the system SHALL return the corresponding `RegisteredUserId`

#### Scenario: Lookup throws for unknown user
- **WHEN** `GetByAccountIdAsync` is called with an Identity user ID that has no persisted `RegisteredUser`
- **THEN** the system SHALL throw `InvalidOperationException`
