<#
.SYNOPSIS
    Clears all user and login data from the Identity database.
    Roles (AspNetRoles, AspNetRoleClaims) are preserved.

.DESCRIPTION
    Deletes rows from all user-related Identity tables inside a single
    SQL Server transaction. Rolls back on any error.

    All four connection parameters are required. Connection strings are
    stored in user secrets and are not read from any settings file.

    Requires PowerShell 5.1+ (uses System.Data.SqlClient, no extra tools needed).

.PARAMETER Server
    SQL Server host and port, e.g. "localhost,1433".

.PARAMETER Database
    Database name.

.PARAMETER Username
    SQL login username.

.PARAMETER Password
    SQL login password.

.EXAMPLE
    .\Clear-IdentityUsers.ps1 -Server "localhost,1433" -Database "TrakmarkDb" -Username "sa" -Password "MyPass1!"
#>
param(
    [Parameter(Mandatory)][string]$Server,
    [Parameter(Mandatory)][string]$Database,
    [Parameter(Mandatory)][string]$Username,
    [Parameter(Mandatory)][string]$Password
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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
    'AspNetUserLogins',
    'AspNetUserClaims',
    'AspNetUserRoles',
    'AspNetUsers',
    'RegisteredUsers'
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
try {
    $conn.Open()
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
