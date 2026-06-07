## ADDED Requirements

### Requirement: Create a meet bound to one sport and one level
The system SHALL create a `Meet` with a `MeetName`, a `MeetDate`, exactly one `CompetitionLevel`, and exactly one `Sport`.

#### Scenario: Create a meet
- **WHEN** a meet is created with a valid name, a date, a competition level, and a sport
- **THEN** the system SHALL create a `Meet` with a new `MeetId`
- **AND** the meet SHALL have no results until results are recorded

### Requirement: Record a student's result with a status
The system SHALL record a `Result` within a `Meet` referencing a `StudentId` and an `Event`, carrying a `Status` ∈ { Finished, DidNotFinish, Disqualified, DidNotStart, NoMark }, a `Tier` ∈ { Varsity, JV, Open }, and an `Order` that reflects entry sequence within the student's results for that meet.

#### Scenario: Tier defaults to Open
- **WHEN** a result is recorded without a tier specified
- **THEN** the system SHALL store the result with tier `Open`

#### Scenario: Record a result with a competitive tier
- **WHEN** a result is recorded with a tier of `Varsity`, `JV`, or `Open`
- **THEN** the system SHALL store that tier on the result

#### Scenario: A finished result requires a mark and a place
- **WHEN** a result is recorded with status `Finished`
- **THEN** the system SHALL require a `Mark` matching the event discipline's mark kind
- **AND** require a `Place`
- **AND** reject the result if either is missing

#### Scenario: A non-finished result carries neither mark nor place
- **WHEN** a result is recorded with status `DidNotFinish`, `Disqualified`, `DidNotStart`, or `NoMark`
- **THEN** the system SHALL store no `Mark` and no `Place`
- **AND** reject the result if a `Mark` or `Place` is supplied

#### Scenario: Results preserve entry order
- **WHEN** multiple results are recorded for a student at one meet
- **THEN** the system SHALL assign each an `Order` reflecting the sequence in which they were entered
- **AND** there SHALL be no separate heat concept

### Requirement: A mark must match the discipline's mark kind
The system SHALL accept a `Performance` mark on a result only if it matches the mark kind declared by the event's `Discipline` (a time discipline accepts a `TimeMark`, a distance discipline accepts a `DistanceMark`).

#### Scenario: Reject a mismatched mark
- **WHEN** a `DistanceMark` is recorded for a result whose discipline declares a time mark kind
- **THEN** the system SHALL reject the result

#### Scenario: A place-only discipline takes a place and no mark
- **WHEN** a finished result is recorded for a place-only discipline
- **THEN** the system SHALL require a `Place`
- **AND** store no `Mark`

### Requirement: A result's event sport must match the meet's sport
When recording a result, the `Meet` SHALL reject any `Event` whose `Sport` differs from the meet's `Sport`.

#### Scenario: Reject a cross-sport event
- **WHEN** a result is recorded on a Track & Field meet for an event whose sport is Cross-Country
- **THEN** the system SHALL reject the result

### Requirement: A meet's level must match the student's enrollment level for the season
The system SHALL allow a result for a student only when the meet's `CompetitionLevel` matches the student's enrollment level for the season whose `SchoolYear` corresponds to the meet's date.

#### Scenario: Reject a level mismatch
- **WHEN** a result is recorded on a Middle School meet for a student whose enrollment for that meet's season is at the High School level
- **THEN** the system SHALL reject the result

#### Scenario: Historical meets validate against the season's enrollment
- **WHEN** a result is recorded on a meet from a past season
- **THEN** the system SHALL validate the meet's level against the student's enrollment for that past season
- **AND** SHALL NOT use the student's current enrollment

### Requirement: Relay results carry the shared team time per student
The system SHALL record a relay event as a separate `Result` for each participating student, each carrying the same team total time as a `TimeMark`, without modeling a shared multi-student entity.

#### Scenario: Each relay leg's student gets the team time
- **WHEN** a relay result is recorded for the participating students
- **THEN** the system SHALL create one result per student, each with the same team total `TimeMark`
- **AND** SHALL NOT store teammate linkage or per-leg splits
