<#
.SYNOPSIS
    Clears all user and login data from the Identity database.
    Roles (AspNetRoles, AspNetRoleClaims) are preserved.

.DESCRIPTION
    Deletes rows from all user-related Identity tables inside a single
    SQL Server transaction. Rolls back on any error.

    Connection string is read from appsettings.Development.json by default.
    Override individual parts with parameters.

    Requires PowerShell 5.1+ (uses System.Data.SqlClient, no extra tools needed).

.PARAMETER Server
    SQL Server host and port. Default: read from appsettings.Development.json.

.PARAMETER Database
    Database name. Default: read from appsettings.Development.json.

.PARAMETER Username
    SQL login username. Default: read from appsettings.Development.json.

.PARAMETER Password
    SQL login password. Default: read from appsettings.Development.json.

.EXAMPLE
    .\Clear-IdentityUsers.ps1
    .\Clear-IdentityUsers.ps1 -Server "myserver,1433" -Password "MyPass1!"
#>
param(
    [string]$Server,
    [string]$Database,
    [string]$Username,
    [string]$Password
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Read connection string from appsettings.Development.json ─────────────────
$settingsPath = "$PSScriptRoot\..\appsettings.Development.json"
$resolved     = Resolve-Path $settingsPath -ErrorAction SilentlyContinue
$settingsPath = if ($resolved) { $resolved.Path } else { $null }

if ($settingsPath) {
    $settings = Get-Content $settingsPath -Raw | ConvertFrom-Json
    $rawCs    = if ($settings.PSObject.Properties['ConnectionStrings'] -and $settings.ConnectionStrings.PSObject.Properties['DefaultConnection']) { $settings.ConnectionStrings.DefaultConnection } else { $null }

    if ($rawCs) {
        # Parse key=value pairs from the connection string
        $csParts = @{}
        $rawCs -split ';' | Where-Object { $_ -match '=' } | ForEach-Object {
            $k, $v = $_ -split '=', 2
            $csParts[$k.Trim().ToLower()] = $v.Trim()
        }

        if (-not $Server)   { $Server   = $csParts['server'] }
        if (-not $Database) { $Database = $csParts['database'] }
        if (-not $Username) { $Username = $csParts['user id'] }
        if (-not $Password) { $Password = $csParts['password'] }
    }
}

# ── Validate ─────────────────────────────────────────────────────────────────
$missing = @()
if (-not $Server)   { $missing += 'Server' }
if (-not $Database) { $missing += 'Database' }
if (-not $Username) { $missing += 'Username' }
if (-not $Password) { $missing += 'Password' }

if ($missing.Count -gt 0) {
    Write-Error "Missing connection parameters: $($missing -join ', '). Provide via params or appsettings.Development.json."
    exit 1
}

# ── Confirm ───────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "  Server   : $Server"   -ForegroundColor Yellow
Write-Host "  Database : $Database" -ForegroundColor Yellow
Write-Host "  User     : $Username" -ForegroundColor Yellow
Write-Host "  Action   : DELETE all users, logins, tokens, claims, passkeys" -ForegroundColor Yellow
Write-Host "  Kept     : AspNetRoles, AspNetRoleClaims" -ForegroundColor Yellow
Write-Host ""
$answer = Read-Host "  Type 'yes' to proceed"
if ($answer -ne 'yes') {
    Write-Host "Aborted." -ForegroundColor Cyan
    exit 0
}

# ── Connect and execute ───────────────────────────────────────────────────────
$cs = "Server=$Server;Database=$Database;User Id=$Username;Password=$Password;TrustServerCertificate=True"

$tables = @(
    'AspNetUserTokens',
    'AspNetUserPasskeys',
    'AspNetUserLogins',
    'AspNetUserClaims',
    'AspNetUserRoles',
    'AspNetUsers'
)

$deleteSql = ($tables | ForEach-Object { "DELETE FROM [$_];" }) -join "`n"

$sql = @"
BEGIN TRANSACTION;
BEGIN TRY
    $deleteSql
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH
"@

Write-Host ""
Write-Host "Clearing user data..." -ForegroundColor Cyan

$conn = New-Object System.Data.SqlClient.SqlConnection($cs)
try {
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $cmd.CommandTimeout = 30
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Transaction committed." -ForegroundColor Green
}
catch {
    Write-Host "FAILED - transaction rolled back." -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
finally {
    $conn.Close()
}

# ── Verify ────────────────────────────────────────────────────────────────────
$conn.Open()
try {
    $results = @()
    foreach ($table in $tables) {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SELECT COUNT(*) FROM [$table]"
        $n = $cmd.ExecuteScalar()
        $results += [PSCustomObject]@{ Table = $table; Rows = $n }
    }

    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM [AspNetRoles]"
    $roleCount = $cmd.ExecuteScalar()
}
finally {
    $conn.Close()
}

Write-Host ""
Write-Host "Done. Row counts after clear:" -ForegroundColor Green
$results | Format-Table -AutoSize
Write-Host "  AspNetRoles (preserved) : $roleCount" -ForegroundColor Green
Write-Host ""
