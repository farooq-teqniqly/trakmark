# Retro — test-improvements (sections 7–9) — 2026-06-10

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| `retrospective.md` line 49 referenced `.claude/agents/openspec-developer.md` — a file that does not exist; the actual agent file is `.claude/agents/developer.md`. A retro runner following this instruction would target the wrong path. | Fixed the stale file name reference to `developer.md`. | `.claude/agents/retrospective.md` |
| Sections 7–9 added XML `<summary>` comments to PersonalBest test methods in `DerivedReadsTests.cs` but not to DistanceMark/TimeMark test methods in `DisciplineAndMarkTests.cs`. CLAUDE.md listed no exemption for test methods, making the rule ambiguous and causing inconsistent application across commits. | Added explicit exemption for test methods (`[Fact]`, `[Theory]`) to the XML docs rule in both CLAUDE.md and config.yaml. The test method name is the specification; a summary would restate it. | `CLAUDE.md`, `openspec/config.yaml` |

## Carry-forward (not yet actionable)

- No reviewer findings were surfaced for sections 7–9 (all 255 tests passed on first run, no post-review fix commits). Nothing to carry forward from review.
