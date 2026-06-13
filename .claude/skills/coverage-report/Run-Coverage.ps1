<#
.SYNOPSIS
    Runs dotnet test with Coverlet coverage collection and emits the Cobertura XML path.

.PARAMETER SolutionPath
    Path to the .slnx or .sln file. Defaults to Trakmark\Trakmark.slnx relative to script root.

.PARAMETER OutputDir
    Directory for coverage output. Defaults to Trakmark.Domain.Tests\TestResults\coverage-analysis\raw.

.PARAMETER Clean
    If set, deletes any previous raw output before running.

.OUTPUTS
    Writes COBERTURA_PATH:<path> to stdout. All other output goes to the host (not captured).
    Exit code mirrors dotnet test exit code (0 = all pass, 1 = test failures but coverage still valid).
#>
[CmdletBinding()]
param(
    [string] $SolutionPath,
    [string] $OutputDir,
    [switch] $Clean
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = & git -C $PSScriptRoot rev-parse --show-toplevel

if (-not $SolutionPath) {
    $SolutionPath = Join-Path $repoRoot "Trakmark" "Trakmark.slnx"
}
if (-not (Test-Path $SolutionPath)) {
    Write-Error "Solution not found: $SolutionPath"
    exit 2
}

if (-not $OutputDir) {
    $OutputDir = Join-Path $repoRoot "Trakmark.Domain.Tests" "TestResults" "coverage-analysis" "raw"
}

if ($Clean -and (Test-Path $OutputDir)) {
    Remove-Item $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

Write-Host "Running: dotnet test $SolutionPath" -ForegroundColor Cyan
Write-Host "Output:  $OutputDir" -ForegroundColor Cyan

dotnet test $SolutionPath `
    --collect:"XPlat Code Coverage" `
    --results-directory $OutputDir `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include="[*]*" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="[*.Tests]*,[*.Test]*,[*Tests]*,[*Test]*,[*.Specs]*,[*.Testing]*" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.SkipAutoProps=true

$testExitCode = $LASTEXITCODE

if ($testExitCode -gt 1) {
    Write-Error "dotnet test failed with exit code $testExitCode (build error)"
    exit $testExitCode
}

$xml = Get-ChildItem -Path $OutputDir -Filter "coverage.cobertura.xml" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $xml) {
    Write-Error "No coverage.cobertura.xml found under $OutputDir"
    exit 3
}

Write-Output "COBERTURA_PATH:$($xml.FullName)"
exit $testExitCode
