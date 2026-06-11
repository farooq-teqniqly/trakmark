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

- [ ] 6.1 Run coverage analysis and confirm branch coverage ≥ 70% and `Event.Equals` CRAP score ≤ 4
