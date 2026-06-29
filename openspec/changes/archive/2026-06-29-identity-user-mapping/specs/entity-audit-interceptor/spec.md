## ADDED Requirements

### Requirement: IAuditableEntity marks entities that record creation metadata
The system SHALL define an `IAuditableEntity` interface with `CreatedByUserId` (string) and `CreatedAt` (DateTimeOffset) properties. Any EF entity that needs creation metadata SHALL implement this interface.

#### Scenario: CityEntity implements IAuditableEntity
- **WHEN** `CityEntity` is inspected at runtime
- **THEN** it SHALL implement `IAuditableEntity`

### Requirement: AuditInterceptor stamps creation metadata on save
The system SHALL provide an `AuditInterceptor` (`SaveChangesInterceptor`) that, before any `SaveChangesAsync`, stamps `CreatedByUserId` and `CreatedAt` on every `IAuditableEntity` entity in `Added` state.

#### Scenario: Added IAuditableEntity is stamped with current user and UTC time
- **WHEN** `SaveChangesAsync` is called
- **AND** one or more `IAuditableEntity` entities are in `Added` state
- **THEN** the interceptor SHALL set `CreatedByUserId` to the `RegisteredUserId.Value` from `ICurrentUserContext`
- **AND** SHALL set `CreatedAt` to the current UTC time

#### Scenario: AuditInterceptor throws when no user is set in context
- **WHEN** `SaveChangesAsync` is called
- **AND** one or more `IAuditableEntity` entities are in `Added` state
- **AND** `ICurrentUserContext.UserId` is `null`
- **THEN** the interceptor SHALL throw `InvalidOperationException`

#### Scenario: Non-IAuditableEntity entities are not affected
- **WHEN** `SaveChangesAsync` is called
- **AND** an entity that does NOT implement `IAuditableEntity` is in `Added` state
- **THEN** the interceptor SHALL not modify that entity

### Requirement: ICurrentUserContext holds the resolved RegisteredUserId for the current scope
The system SHALL provide a scoped `ICurrentUserContext` service with a nullable `RegisteredUserId?` property that authorized Blazor components set in `OnInitializedAsync` after resolving the user via `IRegisteredUserLookupService`.

#### Scenario: Context UserId is null before component sets it
- **WHEN** `ICurrentUserContext` is first resolved from DI in a new scope
- **THEN** `UserId` SHALL be `null`

#### Scenario: Context UserId is available after component sets it
- **WHEN** a component sets `ICurrentUserContext.UserId` to a valid `RegisteredUserId`
- **THEN** the same scoped instance SHALL return that value on subsequent reads
