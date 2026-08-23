[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Join-Path $PSScriptRoot '..')
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path $RepositoryRoot).Path
$skillNames = @(
    'systematic-debugging',
    'test-driven-development',
    'verification-before-completion',
    'security-review',
    'release-notes',
    'structured-pr-review'
)
$requiredSections = @('## Purpose', '## Boundaries', '## Workflow', '## Output', '## Examples')
$errors = [System.Collections.Generic.List[string]]::new()

foreach ($skillName in $skillNames) {
    $skillDirectory = Join-Path $root (Join-Path '.github\skills' $skillName)
    $skillPath = Join-Path $skillDirectory 'SKILL.md'

    if (-not (Test-Path -LiteralPath $skillPath -PathType Leaf)) {
        $errors.Add("Missing $skillPath")
        continue
    }

    $content = Get-Content -LiteralPath $skillPath -Raw
    $frontmatterMatch = [regex]::Match($content, '(?s)\A---\r?\n(.*?)\r?\n---(?:\r?\n|\z)')
    if (-not $frontmatterMatch.Success) {
        $errors.Add("${skillName}: missing YAML frontmatter")
        continue
    }

    $frontmatter = $frontmatterMatch.Groups[1].Value
    $nameMatch = [regex]::Match($frontmatter, '(?m)^name:\s*(.+?)\s*$')
    $descriptionMatch = [regex]::Match($frontmatter, '(?m)^description:\s*(.+?)\s*$')

    if (-not $nameMatch.Success) {
        $errors.Add("${skillName}: missing name")
    } elseif ($nameMatch.Groups[1].Value.Trim().Trim('"') -cne $skillName) {
        $errors.Add("${skillName}: name does not match directory")
    }

    if (-not $descriptionMatch.Success) {
        $errors.Add("${skillName}: missing description")
    } else {
        $description = $descriptionMatch.Groups[1].Value.Trim().Trim('"')
        if ($description.Length -lt 20 -or $description.Length -gt 1024) {
            $errors.Add("${skillName}: description length is outside 20..1024 characters")
        }
        if ($description -notmatch '^Use when\b') {
            $errors.Add("${skillName}: description must start with 'Use when'")
        }
    }

    foreach ($section in $requiredSections) {
        if ($content -notmatch [regex]::Escape($section)) {
            $errors.Add("${skillName}: missing section $section")
        }
    }

    if ($content -match '(?i)TODO|TBD|fill this in') {
        $errors.Add("${skillName}: contains an unfinished marker")
    }

    if ($content -match '(?i)workshop/evaluation|expected-results|adversarial-cases') {
        $errors.Add("${skillName}: references facilitator-only evaluation material")
    }

    $commandLines = $content -split '\r?\n' | Where-Object { $_ -match '^\s*(```|\$|(?:git|dotnet|npm|node|python|pwsh)\b)' }
    foreach ($line in $commandLines) {
        if ($line -match '(?i)\b(?:gh|az\s+(?:repos|devops)|curl|wget|Invoke-(?:RestMethod|WebRequest))\b|@azure-devops/mcp') {
            $errors.Add("${skillName}: contains a mandatory platform or network command: $($line.Trim())")
        }
    }

    if ($content -match '(?i)\bMCP\b' -and $content -notmatch '(?i)optional|fallback|absent|unavailable') {
        $errors.Add("${skillName}: MCP reference lacks an optional fallback")
    }

    $localLinks = [regex]::Matches($content, '\]\((?!https?://|#)([^)]+)\)')
    foreach ($link in $localLinks) {
        $target = $link.Groups[1].Value.Split('#')[0]
        if ([string]::IsNullOrWhiteSpace($target)) {
            continue
        }
        $targetPath = Join-Path $skillDirectory $target
        if (-not (Test-Path -LiteralPath $targetPath)) {
            $errors.Add("${skillName}: broken local reference $target")
        }
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output "Validated $($skillNames.Count) completed reference skills:"
$skillNames | ForEach-Object { Write-Output "- $_" }
Write-Output 'PASS'
