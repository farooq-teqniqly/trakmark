# PR #2 Review — Auto-register Google OAuth users, remove unused scaffolding

**Branch:** `google-oauth-auto-registration`
**Reviewed commit:** `254efcd`
**Reviewer:** Claude (via pr-review skill)
**Date:** 2026-05-30

---

## Summary of Findings

| Severity | Tag | Topic | Location | Source |
|----------|-----|-------|----------|--------|
| ~~🔴 High~~ | ~~Bug/Correctness~~ | ~~Existing user duplicate-create failure~~ — ✅ by-design: local accounts without external logins impossible | `ExternalLogin.razor:99-107` | Qodo, Copilot |
| ~~🟠 Medium~~ | ~~Bug/Reliability~~ | ~~`AddToRoleAsync` result unchecked~~ — ✅ resolved `254efcd` | `ExternalLogin.razor:138` | Copilot |
| ~~🟠 Medium~~ | ~~UX/Reliability~~ | ~~Failure paths strand user on transitional page~~ — ✅ resolved `254efcd` | `ExternalLogin.razor:123,131` | Claude |
| ~~🟡 Low~~ | ~~Doc~~ | ~~Stale XML doc on endpoint extensions~~ — ✅ resolved `254efcd` | `IdentityComponentsEndpointRouteBuilderExtensions.cs:15` | Claude |
| ~~🟡 Low~~ | ~~Doc~~ | ~~Archived proposal/design scope inaccurate~~ — ✅ resolved `a73c259` | `proposal.md`, `design.md` | Copilot |
| ℹ️ Info | PathBase | Qodo flags `RedirectTo(ReturnUrl)` as PathBase-unaware — assessed as likely non-issue for `IdentityRedirectManager` | `ExternalLogin.razor:92,146` | Qodo |

---

## Existing Review Comments (Deduplicated Index)

| Reviewer | Thread | Summary | Status |
|----------|--------|---------|--------|
| Qodo | `ExternalLogin.razor:99-107` | OAuth fails for existing email — no `FindByEmailAsync` before `CreateAsync` | ✅ By-design |
| Qodo | Issue comment | PathBase-unaware `RedirectTo("/")` | Open (assessed non-issue) |
| Copilot | `ExternalLogin.razor:134` | `AddToRoleAsync` result ignored | ✅ Resolved `254efcd` |
| Copilot | `proposal.md:22-24` | Impact section says "only file changed" — inaccurate | ✅ Resolved `a73c259` |
| Copilot | `design.md:35` | Migration note says "Single file change" — inaccurate | ✅ Resolved `a73c259` |

---

## TL;DR

Removes the unnecessary email entry form from Google OAuth registration and auto-creates accounts using the verified Google email claim. Also purges ~1,900 lines of unused scaffold (2FA, passkeys, external login management). The core logic is correct and the deletion work is clean. Two bugs in `ExternalLogin.razor` need fixing before merge.

---

## Architecture / Data Flow

```
Google OAuth callback
        │
        ▼
ExternalLogin.razor → OnLoginCallbackAsync
        │
        ├─ ExternalLoginSignInAsync succeeds → RedirectTo(ReturnUrl)
        ├─ IsLockedOut → RedirectTo("Account/Lockout")
        ├─ No email claim → RedirectToWithStatus("Account/Login", error)
        └─ New user path → CreateAndSignInUserAsync(email, info)
                                │
                                ├─ CreateAsync fails → RedirectToWithStatus("Account/Login", error)
                                ├─ AddLoginAsync fails → RedirectToWithStatus("Account/Login", error)
                                ├─ AddToRoleAsync fails → throw InvalidOperationException
                                └─ SignInAsync → RedirectTo(ReturnUrl)
```

---

## File-by-File Notes

### `Trakmark/Components/Account/Pages/ExternalLogin.razor`

**What changed:** Removed email form, `InputModel`, `OnValidSubmitAsync`. Added `CreateAndSignInUserAsync` helper. Both redirects now go to `"/"`.

#### 🔴 [New] Existing user with unlinked external login will hit duplicate-create failure

`ExternalLogin.razor:99-107` — [Already raised by: Qodo, Copilot]

`ExternalLoginSignInAsync` fails when the user has a local account *but no external login row* for this provider. This is a real runtime state (e.g., account pre-provisioned, or created in a future admin flow). The code goes straight to `CreateAndSignInUserAsync`, which calls `UserManager.CreateAsync` → duplicate username/email error → `message` is set, user stranded.

**Fix:**

```csharp
var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
if (email is null) { /* redirect to error */ return; }

// Check for existing user before attempting creation
var existingUser = await UserManager.FindByEmailAsync(email);
if (existingUser is not null)
{
    var linkResult = await UserManager.AddLoginAsync(existingUser, externalLoginInfo);
    if (linkResult.Succeeded)
    {
        await SignInManager.SignInAsync(existingUser, isPersistent: false, externalLoginInfo.LoginProvider);
        RedirectManager.RedirectTo("/");
    }
    else
    {
        RedirectManager.RedirectToWithStatus("Account/Login",
            "Unable to link your Google account. Please contact support.", HttpContext);
    }
    return;
}

await CreateAndSignInUserAsync(email, externalLoginInfo);
```

---

#### 🟠 [New] `CreateAsync`/`AddLoginAsync` failures leave user stranded on transitional page

`ExternalLogin.razor:119-130` — Claude finding.

On failure, `message` is set and the method returns. But the page renders as a transitional "Completing registration..." page — users see the error in `<StatusMessage>` with no way to recover except navigating back manually.

**Fix:** Replace `message = ...` + `return` with `RedirectManager.RedirectToWithStatus("Account/Login", $"Error: ...", HttpContext)` for both failure branches.

---

#### 🟠 `AddToRoleAsync` result unchecked

`ExternalLogin.razor:134` — [Already raised by: Copilot]

```csharp
await UserManager.AddToRoleAsync(user, "User");  // result ignored
await SignInManager.SignInAsync(...);
```

If role assignment fails, the user signs in without the "User" role and will likely hit authorization failures on protected routes.

**Context:** `WebApplicationExtensions.cs:16-20` seeds both "Admin" and "User" roles at startup, so role-missing failure is unlikely in practice — but not impossible (startup failure, DB issue during seeding). Should check the result and redirect with status on failure.

---

#### ℹ️ PathBase redirect concern (Qodo) — likely non-issue

`ExternalLogin.razor:90, 136` — [Already raised by: Qodo]

Qodo flags `RedirectManager.RedirectTo("/")` as PathBase-unaware. This is valid for `TypedResults.LocalRedirect` (used in the minimal API endpoints), but `IdentityRedirectManager` wraps `NavigationManager.NavigateTo`, which in Blazor SSR resolves paths relative to the app's base URI (which includes PathBase). The `"/"` value should resolve correctly.

**Recommendation:** Verify under a subpath deployment if this ever applies. Low risk for current single-root deployment.

---

### `Trakmark/Components/Account/IdentityComponentsEndpointRouteBuilderExtensions.cs`

**What changed:** Removed `/PasskeyCreationOptions`, `/PasskeyRequestOptions`, `/Manage/LinkExternalLogin` endpoints. Removed unused usings.

#### 🟡 [New] Stale XML doc summary

`IdentityComponentsEndpointRouteBuilderExtensions.cs:15-17`:

```csharp
/// Maps the /Account endpoint group, including external login, logout, passkey creation/request,
/// manage external login linking, and personal data download endpoints.
```

"passkey creation/request" and "manage external login linking" were removed. Update to:

```csharp
/// Maps the /Account endpoint group: external login, logout, and personal data download.
```

---

### Deleted files (18 razor + 5 C#)

Clean. All deletions are consistent — no orphaned references remain. `ManageNavMenu.razor` correctly trimmed to Profile + Personal data only. `App.razor` passkey JS removed. `ServiceCollectionExtensions.cs` `IEmailSender` registration removed.

---

### Archived OpenSpec artifacts (`openspec/changes/archive/`)

**Copilot flags:** `proposal.md` Impact says "only file changed" / "No API changes"; `design.md` Migration says "Single file change". These are accurate for the original scoped proposal but stale after the scope expanded to include scaffold removal.

Low priority since the archive is historical, but worth updating for traceability.

---

## Checklist

### Functionality
- [x] Core OAuth auto-registration flow correct
- [x] Existing user with unlinked external login — ✅ by-design: local accounts without external logins are impossible; Google OAuth is the only account creation path
- [x] No email claim error path correct
- [x] Lockout path preserved
- [x] Deleted pages/endpoints leave no broken references
- [x] `ReturnUrl` restored — existing and new users redirect to `ReturnUrl` after auth (`254efcd`)

### Thread Safety
- [x] No shared mutable state; all per-request
- [x] No race conditions introduced

### Performance
- [x] No blocking operations added
- [x] No N+1 queries introduced

### Code Quality
- [x] Follows project patterns (source-generated logging, no direct `LogInformation`)
- [x] `CreateAndSignInUserAsync` helper clean and focused
- [x] `AddToRoleAsync` result checked — throws `InvalidOperationException` on failure (`254efcd`)
- [x] Failure paths redirect to login with status message (`254efcd`)
- [x] Stale XML doc on endpoint extensions class fixed (`254efcd`)
- [x] Archived proposal/design scope descriptions corrected (`a73c259`)

### Simplification / Refactoring
- [x] `OnValidSubmitAsync` correctly eliminated — no duplication introduced
- [x] No copy-paste

### Testing
- [ ] No tests for the new auto-registration flow
- [ ] No tests for failure paths (duplicate email, role assignment failure, no email claim)

---

## Questions for Author

1. Should the `AddToRoleAsync` failure redirect to login with an error, or throw (since it would indicate a seeding failure that shouldn't be silently swallowed)?
2. Is there an admin flow planned that could create local accounts without an external login row? If yes, the existing-user fix becomes more urgent.
3. Are there plans for protected routes that would benefit from restoring `ReturnUrl`? If so, now is a good time before users accumulate.

---

## References

- `Trakmark/Components/Account/Pages/ExternalLogin.razor`
- `Trakmark/Components/Account/IdentityComponentsEndpointRouteBuilderExtensions.cs`
- `Trakmark/Extensions/WebApplicationExtensions.cs` (role seeding)
- `openspec/specs/google-oauth-auto-registration/spec.md`
