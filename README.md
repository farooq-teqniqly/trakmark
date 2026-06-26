# Trakmark

## Developer Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) or [Podman](https://podman.io/getting-started/installation)
- A Google account with access to [Google Developer Console](https://console.developers.google.com/)

---

### 1. SQL Server via Docker

Create a `.env` file in the repo root (gitignored — never commit it):

```
MSSQL_SA_PASSWORD=<your-strong-password>
```

Start SQL Server:

```bash
docker compose up -d
```

This runs SQL Server 2025 on `localhost,1433` with a persistent volume (`sqlserver_data`).

---

### 2. User Secrets

All secrets go in user secrets — never in `appsettings.json`.

```powershell
cd Trakmark
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=Trakmark;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"
dotnet user-secrets set "Authentication:Google:ClientId" "<your-client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<your-client-secret>"
```

---

### 3. Google OAuth

1. Open [Google Developer Console](https://console.developers.google.com/) and create a project.
2. Go to **APIs & Services → Credentials → Create Credentials → OAuth 2.0 Client ID**.
3. Application type: **Web application**.
4. Add an authorized redirect URI: `https://localhost:7214/signin-google`
5. Copy the **Client ID** and **Client Secret** into user secrets (step 2).

---

### 4. Database Migration

Set the design-time connection string environment variable, then run migrations:

```powershell
$env:TRAKMARK_DESIGN_TIME_CONNSTR = "Server=localhost,1433;Database=Trakmark;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"
dotnet ef database update --project Trakmark --startup-project Trakmark
```

To scaffold a new migration:

```powershell
dotnet ef migrations add <MigrationName> --project Trakmark --startup-project Trakmark
```

---

### 5. Run the App

```powershell
dotnet run --project Trakmark --launch-profile https
```

App is available at `https://localhost:7214`.

---

### 6. Grant Admin Role

After logging in for the first time, assign your account the Admin role directly in the database:

```sql
DECLARE @UserId  NVARCHAR(450) = (SELECT Id FROM AspNetUsers  WHERE Email = '<your-email>');
DECLARE @RoleId  NVARCHAR(450) = (SELECT Id FROM AspNetRoles  WHERE Name  = 'Admin');
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
```

Run this against the `Trakmark` database on `localhost,1433` using any SQL client (SSMS, Azure Data Studio, etc.).

---

### 7. OpenTelemetry — Aspire Dashboard (optional)

Run the Aspire standalone dashboard to view traces and logs locally:

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 mcr.microsoft.com/dotnet/aspire-dashboard:latest
```

Open `http://localhost:18888` to view telemetry. See [Aspire standalone dashboard docs](https://aspire.dev/dashboard/standalone/) for configuration options.
