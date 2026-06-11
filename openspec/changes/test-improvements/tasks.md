## 1. Reorganize Test Files Into Subfolders

- [x] 1.1 Create subfolders `Aggregates/`, `Catalog/`, `Ids/`, `Services/`, `ValueObjects/` under `Trakmark.Domain.Tests/`
- [x] 1.2 Move `CareerTests.cs`, `StudentCareerTests.cs`, `MeetResultTests.cs`, `SchoolAggregateTests.cs`, `RegisteredUserTests.cs` to `Aggregates/`; update namespace to `Trakmark.Domain.Tests.Aggregates`
- [x] 1.3 Move `DisciplineAndMarkTests.cs` to `Catalog/`; update namespace to `Trakmark.Domain.Tests.Catalog`
- [x] 1.4 Move `StronglyTypedIdTests.cs` to `Ids/`; update namespace to `Trakmark.Domain.Tests.Ids`
- [x] 1.5 Move `DomainServicesTests.cs` and `DerivedReadsTests.cs` to `Services/`; update namespace to `Trakmark.Domain.Tests.Services`
- [x] 1.6 Move `ValueObjectTests.cs` to `ValueObjects/`; update namespace to `Trakmark.Domain.Tests.ValueObjects`
- [x] 1.7 Run `dotnet test` — all 160 existing tests must still pass

## 2. Add Catalog Equality Tests

- [x] 2.1 In `Catalog/DisciplineAndMarkTests.cs`, add `[Theory]` covering `Discipline` equality: same identity key → equal; different factory method → not equal; null → false; wrong type → false; `==`/`!=`; `GetHashCode` consistent; `ToString` returns name
- [x] 2.2 Add `[Theory]` covering `Event` equality: same discipline + sport → equal; same discipline, different sport → not equal; different discipline, same sport → not equal (exercises each short-circuit branch); null → false; wrong type → false; `==`/`!=`
- [x] 2.3 Add `[Theory]` covering `Tier` equality: same singleton → equal; different singleton → not equal; null → false; wrong type → false; `==`/`!=`; `ToString`
- [x] 2.4 Add `[Theory]` covering `HurdleHeight` equality: same singleton → equal; different singleton → not equal; null → false; `==`/`!=`; `ToString`
- [x] 2.5 Add `[Theory]` covering `ImplementWeight` equality: same singleton → equal; different singleton → not equal; null → false; `==`/`!=`; `ToString`
- [x] 2.6 Add `[Theory]` covering `Placement` equality: same rank → equal; different rank → not equal; null → false; wrong type → false; `==`/`!=`; `ToString`
- [x] 2.7 Run `dotnet test` — all tests pass

## 3. Add Value Object Equality Tests

- [x] 3.1 In `ValueObjects/ValueObjectTests.cs`, add `[Theory]` covering `MeetName` equality: same value (case-insensitive) → equal; different value → not equal; null → false; wrong type → false; `==`/`!=`; `GetHashCode` consistent; `ToString`
- [x] 3.2 Add `[Theory]` covering `PersonName` equality: same/different/null/wrong-type/`==`/`!=`/`GetHashCode`/`ToString`
- [x] 3.3 Add `[Theory]` covering `SchoolName` equality: same/different/null/wrong-type/`==`/`!=`/`GetHashCode`/`ToString`
- [x] 3.4 Add `[Theory]` covering `TeamName` equality: same/different/null/wrong-type/`==`/`!=`/`GetHashCode`/`ToString`
- [x] 3.5 Add `[Theory]` covering `GradeLevel` equality: same singleton → equal; different singleton → not equal; null → false; wrong type → false; `==`/`!=`; `ToString`
- [x] 3.6 Add `[Theory]` covering `CompetitionLevel` equality: same/different singletons; null; wrong type; `==`/`!=`; `ToString`
- [x] 3.7 Add `[Theory]` covering `Sport` equality: same/different singletons; null; wrong type; `==`/`!=`; `ToString`
- [x] 3.8 Run `dotnet test` — all tests pass

## 4. Add Enrollment Equality Tests

- [x] 4.1 Create `Aggregates/EnrollmentTests.cs` (namespace `Trakmark.Domain.Tests.Aggregates`) with `[Theory]` covering `Enrollment` equality: same all-fields → equal; differ by `SchoolId` → not equal; differ by `SchoolYear` → not equal; differ by `GradeLevel` → not equal; null → false; wrong type → false; `==`/`!=`; `GetHashCode` consistent
- [x] 4.2 Run `dotnet test` — all tests pass

## 5. Add ID TryParse Failure-Path Tests

- [x] 5.1 In `Ids/StronglyTypedIdTests.cs`, add `[Theory]` with one malformed input per type (`MeetId`, `RegisteredUserId`, `SchoolId`, `TeamId`) — wrong prefix, wrong length, or invalid charset — asserting `TryParse` returns `false`
- [x] 5.2 Run `dotnet test` — all tests pass

## 6. Verify Coverage Target

- [x] 6.1 Run coverage analysis and confirm branch coverage ≥ 70% and `Event.Equals` CRAP score ≤ 4

## 7. Add DistanceMark and TimeMark Equality Tests

- [x] 7.1 In `Catalog/DisciplineAndMarkTests.cs`, add `[Theory]` covering `DistanceMark` equality: same value → equal; different value → not equal; null → false; wrong type → false; `==`/`!=`; `GetHashCode` consistent; `ToString` non-empty
- [x] 7.2 Add `[Theory]` covering `TimeMark` equality: same value → equal; different value → not equal; null → false; wrong type → false; `==`/`!=`; `GetHashCode` consistent; `ToString` non-empty
- [x] 7.3 Run `dotnet test` — all tests pass

## 8. Close PersonalBest Branch Gap

- [x] 8.1 In `Services/DomainServicesTests.cs` (or a new `Services/BestMarksServiceTests.cs`), add `[InlineData]`/`[Theory]` rows for `PersonalBest`: empty collection, collection with non-matching discipline, collection where `IsBetterThan` returns `false` for all
- [x] 8.2 Run `dotnet test` — all tests pass

## 9. Add SchoolYear Comparison Operator Tests

- [x] 9.1 In `ValueObjects/ValueObjectTests.cs`, add `[Theory]` covering `SchoolYear` `>=` and `<=`: equal years satisfy both; later year satisfies `>` and `>=`; earlier year satisfies `<` and `<=`
- [x] 9.2 Run `dotnet test` — all tests pass

## 10. Verify Updated Coverage

- [x] 10.1 Run coverage analysis and confirm line coverage ≥ 94% and branch coverage ≥ 85%

## 11. Close CompetitionLevelMatchService.IsLevelMatch Branch Gap

- [x] 11.1 In `Services/`, add a `[Theory]` case exercising the school-level lookup branch in `IsLevelMatch`; assert correct return value
- [x] 11.2 Run `dotnet test` — all tests pass

## 12. Cover Remaining Catalog Boilerplate

- [x] 12.1 In `Catalog/DisciplineAndMarkTests.cs`, extend existing `[Theory]` tests for `Event`, `HurdleHeight`, `ImplementWeight`, `MarkKind`, `Placement`, `Tier` to cover `Equals(object)` wrong-type path, `GetHashCode` consistency, and `ToString` non-empty
- [x] 12.2 Run `dotnet test` — all tests pass

## 13. Close DomainId.IsValid Branch Gap

- [x] 13.1 In `Ids/StronglyTypedIdTests.cs` (or a new `Ids/DomainIdTests.cs`), add `[InlineData]` row for `null` or empty string input to `IsValid`; assert returns `false`
- [x] 13.2 Run `dotnet test` — all tests pass

## 14. Verify Final Coverage

- [x] 14.1 Run coverage analysis and confirm line coverage ≥ 97% and branch coverage ≥ 88% — line PASS (98.9%), branch FAIL (86.9%, target 88%)

## 15. Close Null-Guard Branch Gaps on Value Object Operators

- [x] 15.1 For each value object and catalog type with `op_Equality`/`op_Inequality` tests, ensure the existing `[Theory]` or `[Fact]` passes `null` as the left-hand operand (e.g. `null == x`) to exercise the null-guard branch. Affected types: all types in `Catalog/`, `ValueObjects/`, `Aggregates/`, and `Ids/` that implement `==`/`!=`. Also add null-left assertions for `IsBetterThan` and `Parse`/`TryParse` helpers where applicable.
- [x] 15.2 Run `dotnet test` — all tests pass

## 16. Verify Final Branch Coverage

- [x] 16.1 Run coverage analysis and confirm branch coverage ≥ 88% — PASS (95.5%)

## 17. Cover Meet.EnforceFinishedInvariant Missing Branch

- [x] 17.1 In `Aggregates/MeetResultTests.cs` (or equivalent), add a test with the specific Event/Performance/Placement combination that exercises the uncovered branch in `EnforceFinishedInvariant`; assert the invariant is enforced correctly
- [x] 17.2 Run `dotnet test` — all tests pass; confirm `EnforceFinishedInvariant` CRAP drops from 8.01 to 8.0

## 18. Cover ID Parse Failure Paths

- [x] 18.1 In `Ids/StronglyTypedIdTests.cs`, add `[Theory]` asserting `Parse` throws for a malformed input on each of `MeetId`, `RegisteredUserId`, `SchoolId`, and `TeamId`
- [x] 18.2 Run `dotnet test` — all tests pass

## 19. Cover IsBetterThan Equal/Worse Branch

- [x] 19.1 In `Catalog/DisciplineAndMarkTests.cs`, add `[Theory]` with wrong-type `Performance` pairs asserting `IsBetterThan` returns `false`
- [x] 19.2 Run `dotnet test` — all tests pass

## 20. Verify Final Coverage

- [ ] 20.1 Run coverage analysis and confirm line coverage ≥ 99% and branch coverage ≥ 97%
