## ADDED Requirements

### Requirement: Domain IDs use a per-type prefixed format
Each domain-generated identifier SHALL have the form `PREFIX-BODY`, where `PREFIX` identifies the aggregate type and `BODY` is six Crockford base32 characters (uppercase `A`–`Z` and `2`–`9`, excluding `0`, `O`, `1`, `I`, `L`). The prefixes are `STU-` (Student), `MEET-` (Meet), `SCH-` (School), `TEAM-` (Team), and `USR-` (RegisteredUser).

#### Scenario: New id matches the type's format
- **WHEN** a new `StudentId` is generated
- **THEN** it SHALL match the pattern `STU-` followed by exactly six Crockford base32 characters

#### Scenario: Each aggregate type uses its own prefix
- **WHEN** new ids are generated for a meet, school, team, and registered user
- **THEN** they SHALL begin with `MEET-`, `SCH-`, `TEAM-`, and `USR-` respectively

### Requirement: Ill-formed identifiers cannot be constructed
The system SHALL reject any attempt to construct or parse a domain identifier whose value does not match that type's prefix and a six-character Crockford base32 body.

#### Scenario: Reject a wrong prefix
- **WHEN** a value `MEET-7F3K9M` is parsed as a `StudentId`
- **THEN** the system SHALL reject it

#### Scenario: Reject an ambiguous or out-of-charset character
- **WHEN** a value `STU-7F3K90` (containing `0`) is parsed as a `StudentId`
- **THEN** the system SHALL reject it

#### Scenario: Reject a wrong-length body
- **WHEN** a value `STU-7F3K9` (five body characters) is parsed as a `StudentId`
- **THEN** the system SHALL reject it

#### Scenario: Identifiers round-trip through text
- **WHEN** a generated identifier is converted to text and parsed back
- **THEN** the parsed identifier SHALL equal the original

### Requirement: UserAccountId is exempt from the domain format
The `UserAccountId` SHALL wrap the external ASP.NET Identity account key unchanged and SHALL NOT be required to match the domain identifier format.

#### Scenario: UserAccountId keeps the external key
- **WHEN** a `UserAccountId` is created from an ASP.NET Identity account key
- **THEN** the system SHALL store the key as-is
- **AND** SHALL NOT apply a domain prefix or Crockford base32 constraint
