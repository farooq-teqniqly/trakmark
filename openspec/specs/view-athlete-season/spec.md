## ADDED Requirements

### Requirement: A season is a student's enrollment
The system SHALL present a student's seasons as the student's enrollments, where the current season is the current enrollment and past seasons are past enrollments, without introducing a separate season type.

#### Scenario: Current season is the current enrollment
- **WHEN** a student's seasons are listed
- **THEN** the system SHALL identify the current season as the enrollment with the latest `SchoolYear`
- **AND** identify past seasons as the remaining enrollments

### Requirement: List a season's events and results in entry order
The system SHALL list, for a given student and season, that student's results whose meet date falls within the season's `SchoolYear`, ordered by each result's entry `Order`.

#### Scenario: Show a student's current season results
- **WHEN** a user views the current season for a student they can see
- **THEN** the system SHALL list the student's results for meets in the current season's `SchoolYear`
- **AND** present them in entry order

#### Scenario: Navigate to a past season
- **WHEN** a user selects a past season for a student
- **THEN** the system SHALL list the student's results for meets in that season's `SchoolYear`

### Requirement: A user sees only students they may view
The system SHALL show a registered user only students in the user's `Following` set, together with any student whose `UserAccountId` equals the user's account.

#### Scenario: Followed students are visible
- **WHEN** a user lists the students they can view
- **THEN** the system SHALL include every student in the user's `Following` set

#### Scenario: Unrelated students are not visible
- **WHEN** a user lists the students they can view
- **THEN** the system SHALL exclude students that are neither followed by the user nor linked to the user's account
