# Trakmark

| Setting | Value |
|---------|-------|
| **Interactivity Mode** | Server |
| **Interactivity Scope** | Per-page |
| **Authentication** | Google OAuth only (ASP.NET Core Identity + External Login) |
| **Roles** | `Admin`, `User` (default) — seeded at startup |
| **Database** | SQL Server via EF Core |
| **Theme** | CSS custom properties (`--cf-theme-*`) in `wwwroot/css/themes.css` |

## Rendering configuration
Per-page Interactive Server with prerendering.
Created with `dotnet new blazor -int Server -au Individual`.

Pages are static SSR by default. Only components that explicitly add `@rendermode InteractiveServer` become interactive.

## Project structure

```
Trakmark/
├── Components/
│   ├── App.razor                        # Root — loads Bootstrap, Bootstrap Icons CDN, Google Fonts, themes.css
│   ├── Routes.razor
│   ├── _Imports.razor
│   ├── Layout/
│   │   ├── MainLayout.razor             # Top nav + main + footer (Bootstrap flex layout)
│   │   ├── TopNavMenu.razor             # Responsive Bootstrap navbar, auth-aware
│   │   ├── TopNavMenu.razor.css         # Scoped nav styles using --cf-theme-* vars
│   │   ├── TopNavFooter.razor           # Simple footer
│   │   └── TopNavFooter.razor.css       # Scoped footer styles
│   ├── Pages/
│   │   ├── Home.razor                   # Landing page
│   │   ├── Error.razor
│   │   └── NotFound.razor
│   └── Account/                         # ASP.NET Core Identity pages (always static SSR)
│       ├── Pages/Login.razor            # Google-only login — shows ExternalLoginPicker
│       ├── Pages/ExternalLogin.razor    # Handles Google callback; auto-confirms email; assigns User role
│       └── ...
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationUser.cs
│   └── Migrations/
├── wwwroot/
│   ├── app.css                          # Global styles — fonts, form validation, Blazor error UI
│   └── css/
│       └── themes.css                   # CSS custom property theme system (--cf-theme-*)
└── Program.cs                           # Google OAuth, roles, Identity config
```

## Adding new components
- Create `.razor` files in `Components/Pages/` for routable pages, `Components/` for shared components.
- New pages are static SSR by default. Add `@rendermode InteractiveServer` only for components needing client-side behavior.
- Static pages can use standard HTML forms with `[SupplyParameterFromForm]`.

## Authentication & roles
- Google OAuth only — no local password accounts. Configure credentials via user secrets (dev) or environment variables (prod):
  ```
  dotnet user-secrets set "Authentication:Google:ClientId" "<id>"
  dotnet user-secrets set "Authentication:Google:ClientSecret" "<secret>"
  ```
- `ExternalLoginSignInAsync` is called with `bypassTwoFactor: true`. This is intentional: Google is the sole
  identity provider and is trusted to enforce its own MFA on the Google account. No app-level TOTP second
  factor is required for OAuth sign-ins.
- `Admin` and `User` roles seeded at startup in `Program.cs`.
- New users get the `User` role automatically on first Google login (in `ExternalLogin.razor`).
- Admin role assignment: add pages/logic to call `UserManager.AddToRoleAsync(user, "Admin")`.

## Theming
- Default theme: yellow (`data-cf-theme="yellow"` on `<body>`).
- Change theme by setting `data-cf-theme="<name>"` on a parent element (or `<body>`).
- Available themes defined in `wwwroot/css/themes.css`: `blue`, `indigo`, `purple`, `green`, `teal`, `gray`, `yellow`, `code-magic`, `pink`, `red`, `orange`, `cyan`.
- Navbar and footer use `var(--cf-theme-850)` background automatically.

## Data access
- Inject EF Core `ApplicationDbContext` directly in components — no HTTP API layer needed.
- `HttpContext` available in static components via `[CascadingParameter]`.

## Don'ts
- Don't add `@rendermode` to Identity/Account pages — they must stay static SSR.
- Don't inject `HttpContext` in interactive components — not available during SignalR circuit.
- Don't add `@rendermode InteractiveServer` to every page — keep read-only content static.
- Don't set `@rendermode` on `<Routes>` in `App.razor` — that makes it global.
