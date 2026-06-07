## ADDED Requirements

### Requirement: Compute season best per discipline
The system SHALL compute a season best for a student and discipline as the best `Finished`, individual (non-relay) result with a present mark, among results whose meet falls within that season's `SchoolYear`, using the direction implied by the discipline's mark kind.

#### Scenario: Best time wins for a time discipline
- **WHEN** a season's finished results for a time discipline are evaluated
- **THEN** the system SHALL select the result with the lowest `TimeMark` as the season best

#### Scenario: Best distance wins for a distance discipline
- **WHEN** a season's finished results for a distance discipline are evaluated
- **THEN** the system SHALL select the result with the highest `DistanceMark` as the season best

#### Scenario: Non-finished results are excluded
- **WHEN** a season contains results with status `DidNotFinish`, `Disqualified`, `DidNotStart`, or `NoMark`
- **THEN** the system SHALL exclude those results from the season best

### Requirement: Compute personal best per discipline
The system SHALL compute a personal best for a student and discipline as the best across all seasons — equivalently, the best of the student's season bests — using the direction implied by the discipline's mark kind.

#### Scenario: Personal best spans seasons
- **WHEN** a student has finished results for a discipline in multiple seasons
- **THEN** the system SHALL select the overall best mark across all seasons as the personal best

#### Scenario: Personal best updates as results are added
- **WHEN** a new finished result better than the current personal best is recorded
- **THEN** the recomputed personal best SHALL reflect the new result
- **AND** the personal best SHALL NOT be stored as a flag on any result

### Requirement: Bests are tier-agnostic
The system SHALL compute personal best and season best without regard to a result's `Tier`; a mark set in any tier is eligible.

#### Scenario: A JV mark can be a personal best
- **WHEN** a student's best finished mark for a discipline was set in a `JV` result
- **THEN** the system SHALL select it as the personal best despite the tier
- **AND** SHALL NOT segment bests by tier

### Requirement: Bests exclude relays and place-only disciplines
The system SHALL NOT compute a personal best or season best for relay events or for place-only disciplines.

#### Scenario: Relay marks are excluded
- **WHEN** a discipline's bests are computed
- **THEN** the system SHALL exclude any relay results from the computation

#### Scenario: Place-only disciplines have no best
- **WHEN** a best is requested for a place-only discipline
- **THEN** the system SHALL report that no measurable best exists
