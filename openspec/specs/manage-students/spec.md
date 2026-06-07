## ADDED Requirements

### Requirement: Add a student creates and follows the record
A registered user SHALL add a student through a single operation that creates the canonical `Student` record and adds the new `StudentId` to the user's `Following` set.

#### Scenario: Adding a student creates and follows it
- **WHEN** a registered user adds a student with a valid person name
- **THEN** the system SHALL create a `Student` with a new `StudentId`
- **AND** add that `StudentId` to the user's `Following` set
- **AND** the student SHALL have an empty career until enrollments are recorded

#### Scenario: Person name must be valid
- **WHEN** a registered user adds a student with an empty or whitespace-only name
- **THEN** the system SHALL reject the operation and create no student

### Requirement: Record a student's career as yearly enrollments
The system SHALL record a student's `Career` as a set of `Enrollment`s, each pairing a `SchoolId`, a `SchoolYear`, and a `GradeLevel`, with no two enrollments sharing the same `SchoolYear`.

#### Scenario: Add an enrollment for a new school year
- **WHEN** an enrollment is added for a `SchoolYear` the student has no enrollment in
- **THEN** the system SHALL add the enrollment to the career
- **AND** the enrollment with the latest `SchoolYear` SHALL be the current enrollment

#### Scenario: Reject a duplicate school year
- **WHEN** an enrollment is added for a `SchoolYear` the student already has an enrollment in
- **THEN** the system SHALL reject the operation and leave the career unchanged

#### Scenario: Grade level is recorded independently
- **WHEN** an enrollment is added with a `GradeLevel`
- **THEN** the system SHALL store the supplied `GradeLevel` on the enrollment without deriving it from the school year or competition level

### Requirement: Unfollow a student
A registered user SHALL be able to unfollow a student, removing the `StudentId` from the user's `Following` set without deleting the `Student` record.

#### Scenario: Unfollow removes the link only
- **WHEN** a registered user unfollows a student they are following
- **THEN** the system SHALL remove the `StudentId` from the user's `Following` set
- **AND** the `Student` record SHALL continue to exist

#### Scenario: Following is idempotent
- **WHEN** a registered user follows a `StudentId` already present in their `Following` set
- **THEN** the `Following` set SHALL remain unchanged with no duplicate

### Requirement: Domain identity is decoupled from authentication
The `Student` aggregate SHALL reference an authenticated account only through an optional `UserAccountId` value, which is unset in the pilot.

#### Scenario: A student has no account link by default
- **WHEN** a student is added
- **THEN** the student's `UserAccountId` SHALL be absent
