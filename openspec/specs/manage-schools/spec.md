## ADDED Requirements

### Requirement: Create a school with a competition level
The system SHALL create a `School` with a `SchoolName` and exactly one `CompetitionLevel` (High School, Middle School, or Elementary School).

#### Scenario: Create a school
- **WHEN** a school is created with a valid name and a competition level
- **THEN** the system SHALL create a `School` with a new `SchoolId`
- **AND** store the supplied name and competition level

#### Scenario: School name must be valid
- **WHEN** a school is created with an empty or whitespace-only name
- **THEN** the system SHALL reject the operation and create no school

### Requirement: A school fields teams by sport
A `School` SHALL hold a catalog of `Team`s, each bound to exactly one `Sport`, representing the sports the school fields.

#### Scenario: Add a team for a sport
- **WHEN** a team is added to a school for a sport the school does not yet field
- **THEN** the system SHALL add a `Team` bound to that sport to the school's catalog

#### Scenario: Roster membership is not stored on the team
- **WHEN** a team is inspected
- **THEN** it SHALL NOT contain a stored list of student members
- **AND** roster membership SHALL be derivable from a student's enrollment plus the sport of the events the student contests
