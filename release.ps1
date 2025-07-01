#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [switch]$DryRun
)

Write-Host "🚀 LoadSharp Release Script" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan
Write-Host ""

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "❌ Invalid version format. Use semantic versioning (e.g., 1.0.0)" -ForegroundColor Red
    exit 1
}

Write-Host "📋 Release Summary:" -ForegroundColor Yellow
Write-Host "   Version: $Version" -ForegroundColor White
Write-Host "   Dry Run: $DryRun" -ForegroundColor White
Write-Host ""

# Check if git is clean
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "❌ Git working directory is not clean. Please commit or stash changes." -ForegroundColor Red
    Write-Host "Uncommitted changes:" -ForegroundColor Yellow
    git status --short
    exit 1
}

# Check if we're on main branch
$currentBranch = git branch --show-current
if ($currentBranch -ne "main") {
    Write-Host "⚠️  Warning: Not on main branch (current: $currentBranch)" -ForegroundColor Yellow
    $confirm = Read-Host "Continue? (y/N)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "❌ Release cancelled" -ForegroundColor Red
        exit 1
    }
}

# Update version in project file
Write-Host "📝 Updating version in LoadSharp.csproj..." -ForegroundColor Yellow
$projectFile = "src/LoadSharp/LoadSharp.csproj"
$content = Get-Content $projectFile -Raw
$newContent = $content -replace '<Version>.*</Version>', "<Version>$Version</Version>"

if ($DryRun) {
    Write-Host "   [DRY RUN] Would update version to $Version" -ForegroundColor Gray
} else {
    Set-Content $projectFile -Value $newContent -Encoding UTF8
    Write-Host "   ✅ Version updated to $Version" -ForegroundColor Green
}

# Run tests to ensure everything is working
Write-Host "🧪 Running tests before release..." -ForegroundColor Yellow
if ($DryRun) {
    Write-Host "   [DRY RUN] Would run full test suite" -ForegroundColor Gray
} else {
    & ".\run-tests.cmd"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed. Release cancelled." -ForegroundColor Red
        
        # Restore original version
        Set-Content $projectFile -Value $content -Encoding UTF8
        Write-Host "   Restored original version" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "   ✅ All tests passed" -ForegroundColor Green
}

# Commit version change
Write-Host "📝 Committing version change..." -ForegroundColor Yellow
if ($DryRun) {
    Write-Host "   [DRY RUN] Would commit: 'Release v$Version'" -ForegroundColor Gray
} else {
    git add $projectFile
    git commit -m "Release v$Version"
    Write-Host "   ✅ Version change committed" -ForegroundColor Green
}

# Create and push tag
Write-Host "🏷️  Creating release tag..." -ForegroundColor Yellow
if ($DryRun) {
    Write-Host "   [DRY RUN] Would create tag: v$Version" -ForegroundColor Gray
    Write-Host "   [DRY RUN] Would push tag to origin" -ForegroundColor Gray
} else {
    git tag "v$Version"
    git push origin "v$Version"
    git push origin $currentBranch
    Write-Host "   ✅ Tag v$Version created and pushed" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎉 Release process completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Next steps:" -ForegroundColor Cyan
Write-Host "   1. 🔄 GitHub Actions will automatically:" -ForegroundColor White
Write-Host "      - Build and test the release" -ForegroundColor Gray
Write-Host "      - Create a GitHub release" -ForegroundColor Gray
Write-Host "      - Publish to NuGet (if configured)" -ForegroundColor Gray
Write-Host "   2. 📋 Monitor the release workflow:" -ForegroundColor White
Write-Host "      https://github.com/your-username/loadsharp/actions" -ForegroundColor Gray
Write-Host "   3. 🎯 Update release notes if needed:" -ForegroundColor White
Write-Host "      https://github.com/your-username/loadsharp/releases/tag/v$Version" -ForegroundColor Gray
Write-Host ""

if ($DryRun) {
    Write-Host "💡 This was a dry run. No changes were made." -ForegroundColor Blue
    Write-Host "   Run without -DryRun to execute the release." -ForegroundColor Blue
}
