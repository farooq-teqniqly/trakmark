## MODIFIED Requirements

### Requirement: City persistence records creation metadata
When a `City` is persisted, the system SHALL record the UTC creation timestamp, the identifier of the user who created it, and assign a unique identifier. Creation metadata is stamped by `AuditInterceptor` — services SHALL NOT set `CreatedByUserId` or `CreatedAt` manually.

#### Scenario: Created date is set on persistence
- **WHEN** a city is persisted
- **THEN** the system SHALL set its created date to the current UTC time

#### Scenario: Creating user is recorded on persistence
- **WHEN** an Admin submits a batch that is persisted
- **THEN** the system SHALL record that Admin's `RegisteredUserId` as the creator of each city in the batch
