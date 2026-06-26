## Why

Schools, school districts, and other future entities need a normalized location reference (City + State) to avoid free-text duplication and enable filtering/lookup. Admins need a way to seed this reference data before School Districts (002) and Schools (003) can be added.

## What Changes

- Add a closed-set `State` value object (50 states + DC, two-letter abbreviation identity), modeled like the existing `Sport`/`CompetitionLevel` value objects.
- Add a `City` domain type (`Name` required, max 100 characters; `State` required) with equality defined by `Name` + `State` (case-insensitive name comparison).
- Add an admin-only "Add Cities" use case: batch entry of 1-100 cities in one form submission, client-side validation as the user types, server-side validation and duplicate checking on save, all-or-nothing persistence (entire batch rejected if any row is invalid or duplicates an existing city).
- Add persistence for `City` (surrogate key, UTC created-date set by the database) via EF Core.

## Capabilities

### New Capabilities
- `manage-cities`: Defines the `State` and `City` domain types and the admin "Add Cities" use case — entry, validation, duplicate prevention, and persistence of city reference data.

### Modified Capabilities
(none — `manage-schools` is unaffected by this change; School's future City/State references are out of scope here)

## Impact

- New project code: `Trakmark.Domain/ValueObjects/State.cs`, `Trakmark.Domain/ValueObjects/City.cs` (plus mirrored test files in `Trakmark.Domain.Tests`).
- New EF Core entity/configuration and migration for `City` persistence in the data-access layer.
- New Blazor admin page ("Add Cities") and nav menu entry under the Admin dropdown, restricted to the Admin role.
- No changes to existing `School` aggregate or `manage-schools` spec.
