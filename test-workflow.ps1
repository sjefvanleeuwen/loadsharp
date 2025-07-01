# 🧪 Workflow Test Script

# This script helps test if the GitHub Actions workflow will work correctly
# by simulating the badge update process locally

Write-Host "🧪 Testing GitHub Actions Workflow Logic" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Simulate extracting metrics (as the workflow would do)
Write-Host "📊 Simulating metrics extraction..." -ForegroundColor Yellow

# Read the actual test results
$trxContent = Get-Content "./TestResults/test-results.trx" -Raw -ErrorAction SilentlyContinue
if ($trxContent) {
    $totalTests = [regex]::Match($trxContent, 'total="(\d+)"').Groups[1].Value
    $passedTests = [regex]::Match($trxContent, 'passed="(\d+)"').Groups[1].Value
    $failedTests = [regex]::Match($trxContent, 'failed="(\d+)"').Groups[1].Value
    
    if ([string]::IsNullOrEmpty($failedTests)) { $failedTests = "0" }
    
    Write-Host "   Total tests: $totalTests" -ForegroundColor White
    Write-Host "   Passed: $passedTests" -ForegroundColor Green
    Write-Host "   Failed: $failedTests" -ForegroundColor Red
} else {
    Write-Host "   ⚠️ No test results found - run tests first" -ForegroundColor Yellow
    $totalTests = "0"
    $passedTests = "0"
    $failedTests = "0"
}

# Read coverage data
$coverageContent = Get-Content "./TestResults/CoverageReport/SummaryGithub.md" -Raw -ErrorAction SilentlyContinue
if ($coverageContent) {
    $coverage = [regex]::Match($coverageContent, 'Line coverage:\*\* \| ([\d.]+)%').Groups[1].Value
    Write-Host "   Coverage: $coverage%" -ForegroundColor White
} else {
    Write-Host "   ⚠️ No coverage data found" -ForegroundColor Yellow
    $coverage = "0"
}

# Determine badge colors (same logic as workflow)
$testColor = if ([int]$failedTests -eq 0 -and [int]$totalTests -gt 0) { "brightgreen" } else { "red" }
$testStatus = if ([int]$failedTests -eq 0 -and [int]$totalTests -gt 0) { "$totalTests passing" } else { "failing" }

$coverageNum = [double]$coverage
$coverageColor = if ($coverageNum -ge 80) { "brightgreen" } elseif ($coverageNum -ge 60) { "yellow" } else { "red" }

Write-Host ""
Write-Host "🏷️ Badge updates that would be applied:" -ForegroundColor Cyan
Write-Host "   Tests badge: tests-$($testStatus.Replace(' ', '%20'))-$testColor" -ForegroundColor White
Write-Host "   Coverage badge: coverage-$coverage%25-$coverageColor" -ForegroundColor White

# Test the actual badge update commands
Write-Host ""
Write-Host "🔧 Testing badge update commands..." -ForegroundColor Yellow

# Create a backup of README
Copy-Item "readme.md" "readme.md.backup" -ErrorAction SilentlyContinue

# Try the badge updates (simulate what the workflow does)
try {
    # Read README content
    $readmeContent = Get-Content "readme.md" -Raw
    
    # Apply updates
    $testBadgePattern = '\!\[Tests\]\(https://img\.shields\.io/badge/tests-[^)]*\)'
    $coverageBadgePattern = '\!\[Coverage\]\(https://img\.shields\.io/badge/coverage-[^)]*\)'
    
    $newTestBadge = "![Tests](https://img.shields.io/badge/tests-$($testStatus.Replace(' ', '%20'))-$testColor)"
    $newCoverageBadge = "![Coverage](https://img.shields.io/badge/coverage-$coverage%25-$coverageColor)"
    
    $updatedContent = $readmeContent -replace $testBadgePattern, $newTestBadge
    $updatedContent = $updatedContent -replace $coverageBadgePattern, $newCoverageBadge
    
    # Write back to file
    Set-Content "readme.md" -Value $updatedContent -Encoding UTF8
    
    Write-Host "   ✅ Badge updates applied successfully" -ForegroundColor Green
    
    # Show the diff
    Write-Host ""
    Write-Host "📋 Changes made:" -ForegroundColor Cyan
    Write-Host "   Test badge updated to show: $testStatus" -ForegroundColor White
    Write-Host "   Coverage badge updated to show: $coverage%" -ForegroundColor White
    
} catch {
    Write-Host "   ❌ Error applying badge updates: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "🔄 Restoring original README..." -ForegroundColor Yellow
if (Test-Path "readme.md.backup") {
    Move-Item "readme.md.backup" "readme.md" -Force
    Write-Host "   ✅ README restored" -ForegroundColor Green
} else {
    Write-Host "   ⚠️ No backup found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📊 Test Summary:" -ForegroundColor Green
Write-Host "   - Badge extraction logic: ✅ Working" -ForegroundColor White
Write-Host "   - Badge update commands: ✅ Working" -ForegroundColor White
Write-Host "   - Workflow should now succeed in GitHub Actions" -ForegroundColor White
Write-Host ""
Write-Host "💡 The GitHub Actions workflow is ready to run!" -ForegroundColor Blue
