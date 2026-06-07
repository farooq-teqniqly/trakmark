# Coverage Analysis — Trakmark.Domain

**Date:** 2026-06-07  
**Tests:** 160 passed, 0 failed  
**Methods:** 208 total, 0 exceed CRAP threshold (30)

## 📊 Summary

| Metric | Value | Threshold | Status |
|--------|-------|-----------|--------|
| Line Coverage | 80.5% | 80% | ✅ |
| Branch Coverage | 58.8% | 70% | ⚠️ Below threshold |
| Total Methods | 208 | — | — |
| Flagged Methods (CRAP > 30) | 0 | — | ✅ |

## 🔥 Risk Hotspots (Top 10 by CRAP Score)

| Rank | Class | Method | File | Complexity | Line Cov | CRAP |
|------|-------|--------|------|-----------|----------|------|
| 1 | `Event` | `Equals(Event)` | Catalog/Event.cs | 4 | 0% | **20.0** ⚠️ |
| 2 | `BestMarksService` | `SeasonBest(...)` | Services/BestMarksService.cs | 12 | 100% | 12.0 |
| 3 | `BestMarksService` | `PersonalBest(...)` | Services/BestMarksService.cs | 10 | 100% | 10.0 |
| 4 | `Meet` | `EnforceFinishedInvariant(...)` | Aggregates/Meet.cs | 8 | 94.4% | 8.01 |
| 5 | `StudentVisibilityService` | `MoveNext()` | Services/StudentVisibilityService.cs | 8 | 100% | 8.0 |
| 6 | `Meet` | `EnforceMarkKindMatch(...)` | Aggregates/Meet.cs | 8 | 100% | 8.0 |
| 7 | `Enrollment` | `Equals(Enrollment)` | Aggregates/Enrollment.cs | 6 | 75% | 6.56 ⚠️ |
| 8 | `DomainId` | `IsValid(string, string)` | Ids/DomainId.cs | 6 | 81.8% | 6.22 ⚠️ |
| 9 | `MeetName` | `Equals(MeetName)` | ValueObjects/MeetName.cs | 2 | 0% | 6.0 ⚠️ |
| 10 | `ImplementWeight` | `Equals(ImplementWeight)` | Catalog/ImplementWeight.cs | 2 | 0% | 6.0 ⚠️ |

## 📉 Coverage Gaps by File

The branch coverage deficit (58.8% vs 70% target) is driven almost entirely by untested equality/operator boilerplate on value objects and catalog types. 91 of 113 flagged methods are zero-coverage.

**Uncovered method clusters:**

| File | Uncovered Methods |
|------|------------------|
| Catalog/Event.cs | Equals(Event), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| Catalog/Discipline.cs | Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| Catalog/Performance.cs | DistanceMark/TimeMark: Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| Catalog/Placement.cs | Equals(Placement), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| Catalog/Tier.cs | Equals(Tier), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| Catalog/HurdleHeight.cs | Equals(HurdleHeight), Equals(object), GetHashCode, op_Equality, op_Inequality |
| Catalog/ImplementWeight.cs | Equals(ImplementWeight), Equals(object), GetHashCode, op_Equality, op_Inequality |
| Catalog/MarkKind.cs | Equals(object), GetHashCode, ToString |
| ValueObjects/MeetName.cs | Equals(MeetName), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| ValueObjects/PersonName.cs | Equals(PersonName), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| ValueObjects/SchoolName.cs | Equals(SchoolName), Equals(object), GetHashCode, op_Equality, op_Inequality |
| ValueObjects/TeamName.cs | Equals(TeamName), Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| ValueObjects/CompetitionLevel.cs | Equals(object), GetHashCode, op_Equality, op_Inequality, ToString |
| ValueObjects/GradeLevel.cs | Equals(object), GetHashCode, op_Equality, op_Inequality |
| ValueObjects/Sport.cs | Equals(object), op_Equality, op_Inequality |
| Aggregates/Enrollment.cs | Equals(object), GetHashCode, op_Equality, op_Inequality |
| Ids/MeetId.cs | TryParse (failure branch) |
| Ids/RegisteredUserId.cs | TryParse (failure branch), .cctor |
| Ids/SchoolId.cs | TryParse (failure branch) |
| Ids/TeamId.cs | TryParse (failure branch) |
| Ids/DomainId.cs | IsValid — 2 lines/2 branches uncovered |

## 🎯 Recommendations

**1. Test `Event.Equals` — highest CRAP uncovered (20)**  
One `[Theory]` with same/different/null cases on `Event.Equals(Event)` drops CRAP from 20 → 4 and unlocks the full `Event` equality cluster. File: `Catalog/Event.cs`.

**2. Sweep value object equality with a shared `[Theory]` pattern**  
`MeetName`, `PersonName`, `SchoolName`, `TeamName`, `GradeLevel`, `CompetitionLevel`, `Sport`, `Placement`, `Tier`, `HurdleHeight`, `ImplementWeight` all have identical untested equality surfaces. One parameterized test class covering equal/not-equal/null/wrong-type would eliminate ~70 zero-coverage methods and push branch coverage above 70%.

**3. Test `TryParse` failure paths on ID types**  
`MeetId`, `RegisteredUserId`, `SchoolId`, `TeamId` each have `TryParse` at 50% branch coverage — the invalid/malformed input branch is never exercised. A single `[Theory]` with valid and invalid inputs covers all four types.

## 📁 Reports

| Type | Path |
|------|------|
| Cobertura XML | `Trakmark.Domain.Tests/TestResults/coverage-analysis/raw/.../coverage.cobertura.xml` |
| Markdown | `Trakmark.Domain.Tests/TestResults/coverage-analysis/coverage-analysis.md` |
| HTML | Not generated (optional — request HTML reports to enable) |
| Text Summary | Not generated (optional — request HTML reports to enable) |
| CSV | Not generated (optional — request HTML reports to enable) |
