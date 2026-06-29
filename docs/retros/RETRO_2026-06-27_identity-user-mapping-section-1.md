# Retro — identity-user-mapping section 1 (Domain persistence infrastructure) — 2026-06-27

## Findings applied

| Evidence | Improvement | File(s) changed |
|----------|-------------|-----------------|
| Reviewer flagged redundant `.IsRequired()` on PK and AccountId in `RegisteredUserConfiguration` — EF Core 10 infers required from nullable context, making the calls harmless noise | Added rule: do not call `.IsRequired()` on non-nullable properties in `IEntityTypeConfiguration<T>` when `Nullable` is enabled | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Reviewer flagged absent FK from `RegisteredUsers.AccountId` to `AspNetUsers.Id` as a design question — intentional per D1 but not documented anywhere, so every future reviewer will repeat the question | Added rule: domain tables must not define FK constraints to Identity schema tables; reference by value only; any intentional exception must be recorded as a named decision in `design.md` | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |
| Developer correctly used `.ValueGeneratedNever()` for the domain-assigned PK but no rule existed to enforce this — omitting it in a future entity would silently cause EF Core to discard the domain-supplied ID | Added rule: always call `.ValueGeneratedNever()` on domain-assigned PKs in `IEntityTypeConfiguration<T>` | `CLAUDE.md`, `openspec/config.yaml`, `.claude/agents/developer.md` |

## Implementation notes

- Section 1 had no spec scenarios and no `.T` task; TDD failing-test-first exemption was correctly applied by both the developer agent and the reviewer.
- The developer agent completed all four tasks (1.1–1.4) in a single pass with no permission denials and no fix rounds — no workflow gaps identified.
- Tasks.md merge (checked vs. unchecked tasks across worktree and target branch) was handled cleanly by the pre-consolidation step — no merge conflict pattern to address.

## Carry-forward (not yet actionable)

- Design.md D1 captures the FK decoupling rationale but is change-scoped. If a standing architecture decision record (ADR) mechanism is introduced, the FK/Identity decoupling decision should be promoted there so it survives across changes.
- The reviewer raised whether a future `AuthorizedComponentBase` Blazor base class could enforce `ICurrentUserContext` population. Design.md mentions this as a mitigation for the "component forgets to set context" risk. No rule change possible until the mechanism exists.
