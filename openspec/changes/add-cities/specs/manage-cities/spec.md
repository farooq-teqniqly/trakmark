## ADDED Requirements

### Requirement: State is a closed set of U.S. states and D.C.
The system SHALL provide a `State` value object representing a fixed set of the 50 U.S. states plus the District of Columbia, each identified by its two-letter abbreviation.

#### Scenario: States are equal by abbreviation
- **WHEN** two `State` instances with the same two-letter abbreviation (regardless of case) are compared
- **THEN** the system SHALL consider them equal

#### Scenario: States with different abbreviations are not equal
- **WHEN** two `State` instances with different two-letter abbreviations are compared
- **THEN** the system SHALL consider them not equal

### Requirement: Create a city with a name and state
The system SHALL create a `City` with a required `Name` (non-empty, maximum 100 characters) and a required `State`.

#### Scenario: Create a valid city
- **WHEN** a city is created with a non-empty name of 100 characters or fewer and a valid state
- **THEN** the system SHALL create a `City` with a new `CityId`, the supplied name, and the supplied state

#### Scenario: City name must not be empty
- **WHEN** a city is created with an empty or whitespace-only name
- **THEN** the system SHALL reject the operation and create no city

#### Scenario: City name must not exceed maximum length
- **WHEN** a city is created with a name longer than 100 characters
- **THEN** the system SHALL reject the operation and create no city

#### Scenario: City state is required
- **WHEN** a city is created with a null state
- **THEN** the system SHALL reject the operation and create no city

### Requirement: Cities are equal by name and state
Two `City` instances SHALL be considered equal when their names match case-insensitively and their states are equal.

#### Scenario: Cities with same name and state are equal
- **WHEN** two `City` instances have the same name (regardless of case) and the same state
- **THEN** the system SHALL consider them equal

#### Scenario: Cities with different states are not equal
- **WHEN** two `City` instances have the same name but different states
- **THEN** the system SHALL consider them not equal

#### Scenario: Cities with different names are not equal
- **WHEN** two `City` instances have different names but the same state
- **THEN** the system SHALL consider them not equal

### Requirement: Admin users can add cities in a batch
An authenticated user in the Admin role SHALL be able to submit a batch of 1 to 100 cities for persistence in a single operation.

#### Scenario: Successful batch save
- **WHEN** an Admin submits a batch of 1 to 100 valid, non-duplicate cities
- **THEN** the system SHALL persist all cities in the batch
- **AND** SHALL display a success notification

#### Scenario: Batch save rejects on any invalid row
- **WHEN** an Admin submits a batch containing at least one city with an invalid name or missing state
- **THEN** the system SHALL persist no cities from the batch
- **AND** SHALL display the validation error to the user

#### Scenario: Batch save rejects on duplicate within the batch
- **WHEN** an Admin submits a batch containing two or more cities that are equal to each other (same name and state)
- **THEN** the system SHALL persist no cities from the batch
- **AND** SHALL display an error indicating the duplicate

#### Scenario: Batch save rejects on duplicate against existing data
- **WHEN** an Admin submits a batch containing a city that is equal to (same name and state as) a city already persisted
- **THEN** the system SHALL persist no cities from the batch
- **AND** SHALL display a save-failed notification

#### Scenario: Non-Admin user cannot access the Add Cities form
- **WHEN** an authenticated user who is not in the Admin role attempts to access the Add Cities form
- **THEN** the system SHALL deny access

### Requirement: City persistence records creation metadata
When a `City` is persisted, the system SHALL record the UTC creation timestamp, the identifier of the user who created it, and assign a unique identifier.

#### Scenario: Created date is set on persistence
- **WHEN** a city is persisted
- **THEN** the system SHALL set its created date to the current UTC time

#### Scenario: Creating user is recorded on persistence
- **WHEN** an Admin submits a batch that is persisted
- **THEN** the system SHALL record that Admin's user identifier as the creator of each city in the batch
