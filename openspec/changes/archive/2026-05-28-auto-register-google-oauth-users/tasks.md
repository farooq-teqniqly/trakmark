## 1. Remove Email Form

- [x] 1.1 Delete `InputModel` class and `[SupplyParameterFromForm] Input` property
- [x] 1.2 Remove `EditForm`, email `InputText`, `ValidationSummary`, and Register button from markup

## 2. Auto-Register on Callback

- [x] 2.1 In `OnLoginCallbackAsync`, after failed `ExternalLoginSignInAsync`: check for `ClaimTypes.Email` claim; if missing, redirect to `Account/Login` with error message
- [x] 2.2 Extract user creation logic from `OnValidSubmitAsync` into a private helper `CreateAndSignInUserAsync(string email, ExternalLoginInfo info)`
- [x] 2.3 Call `CreateAndSignInUserAsync` directly from `OnLoginCallbackAsync` when new user + email claim present
- [x] 2.4 Delete `OnValidSubmitAsync` (no longer needed)

## 3. Fix Redirects

- [x] 3.1 Change existing-user redirect (line 116) from `ReturnUrl` to `"/"`
- [x] 3.2 Change new-user redirect in `CreateAndSignInUserAsync` to `"/"`
