[CmdletBinding()]
param(
    [Parameter()]
    [string]$RepositoryRoot = (Get-Location).Path,

    [Parameter()]
    [switch]$RunTests
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryPath = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$webPath = Join-Path $repositoryPath 'src/web'
$solutionPath = Join-Path $repositoryPath 'src/NordicBike.Portal.slnx'

if (-not (Test-Path -LiteralPath $webPath)) {
    throw "Expected NordicBike web project at '$webPath'."
}

$rules = @(
    @{ Id = 'NB001'; Pattern = '\bDateTime(Offset)?\.(Now|UtcNow)\b'; Message = 'Use DemoClock for application time.' },
    @{ Id = 'NB002'; Pattern = '\.Result\b|\.Wait\(\)|GetAwaiter\(\)\.GetResult\(\)'; Message = 'Do not synchronously block asynchronous work.' },
    @{ Id = 'NB003'; Pattern = '\bConsole\.WriteLine\s*\('; Message = 'Use structured logging or PortalAudit instead of Console.WriteLine.' }
)

$candidates = foreach ($file in Get-ChildItem -LiteralPath $webPath -Recurse -File -Filter '*.cs' | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }) {
    $lineNumber = 0
    foreach ($line in Get-Content -LiteralPath $file.FullName) {
        $lineNumber++
        foreach ($rule in $rules) {
            if ($rule.Id -eq 'NB003' -and $file.Name -eq 'StartupReporter.cs') {
                continue
            }
            if ($line -match $rule.Pattern) {
                [pscustomobject]@{
                    rule = $rule.Id
                    file = [IO.Path]::GetRelativePath($repositoryPath, $file.FullName).Replace('\', '/')
                    line = $lineNumber
                    evidence = $line.Trim()
                    message = $rule.Message
                }
            }
        }
    }
}

$checks = @([pscustomobject]@{
        command = 'Static NordicBike standards scan'
        result = if ($candidates) { 'candidates_found' } else { 'passed' }
        exitCode = if ($candidates) { 1 } else { 0 }
    })

if ($RunTests) {
    $testOutput = & dotnet test $solutionPath --nologo 2>&1
    $testExitCode = $LASTEXITCODE
    $checks += [pscustomobject]@{
        command = "dotnet test $solutionPath --nologo"
        result = if ($testExitCode -eq 0) { 'passed' } else { 'failed' }
        exitCode = $testExitCode
        output = ($testOutput | Out-String).Trim()
    }
}

[pscustomobject]@{
    repository = $repositoryPath
    solution = 'src/NordicBike.Portal.slnx'
    candidates = @($candidates)
    checks = $checks
    reviewReminder = 'Verify each candidate in source context before reporting it as a finding.'
} | ConvertTo-Json -Depth 5

if ($candidates -or ($RunTests -and $testExitCode -ne 0)) {
    exit 1
}