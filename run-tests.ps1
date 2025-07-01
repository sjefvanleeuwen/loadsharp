#!/usr/bin/env pwsh

# LoadSharp Test Pipeline Script
# Simulates the GitHub Actions pipeline locally

Write-Host "🚀 LoadSharp Test Pipeline" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

# Clean previous results
Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}

# Restore dependencies
Write-Host "📦 Restoring dependencies..." -ForegroundColor Yellow
dotnet restore LoadSharp.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restore dependencies" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build LoadSharp.sln --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to build solution" -ForegroundColor Red
    exit 1
}

# Run tests with coverage
Write-Host "🧪 Running tests with coverage..." -ForegroundColor Yellow
dotnet test LoadSharp.sln --configuration Release --no-build --verbosity normal --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage" --results-directory ./TestResults
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed" -ForegroundColor Red
    exit 1
}

# Generate coverage report
Write-Host "📊 Generating coverage report..." -ForegroundColor Yellow
$reportGeneratorInstalled = Get-Command reportgenerator -ErrorAction SilentlyContinue
if (-not $reportGeneratorInstalled) {
    Write-Host "⚠️  ReportGenerator not found, installing..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

& reportgenerator "-reports:./TestResults/*/coverage.cobertura.xml" "-targetdir:./TestResults/CoverageReport" "-reporttypes:Html;MarkdownSummaryGithub;Cobertura" "-verbosity:Info"

# Generate test report
Write-Host "📋 Generating test report..." -ForegroundColor Yellow
dotnet run --project tools/TestReportGenerator/TestReportGenerator.csproj --configuration Release -- --input "./TestResults/test-results.trx" --output "./docs/test-results.md" --coverage "./TestResults/CoverageReport/SummaryGithub.md"
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to generate test report" -ForegroundColor Red
    exit 1
}

# Extract metrics for badge updates
Write-Host "🏷️  Extracting metrics for badges..." -ForegroundColor Yellow
$trxContent = Get-Content "./TestResults/test-results.trx" -Raw
$totalTests = [regex]::Match($trxContent, 'total="(\d+)"').Groups[1].Value
$passedTests = [regex]::Match($trxContent, 'passed="(\d+)"').Groups[1].Value
$failedTests = [regex]::Match($trxContent, 'failed="(\d+)"').Groups[1].Value

if ([string]::IsNullOrEmpty($failedTests)) { $failedTests = "0" }

$coverageContent = ""
if (Test-Path "./TestResults/CoverageReport/SummaryGithub.md") {
    $coverageContent = Get-Content "./TestResults/CoverageReport/SummaryGithub.md" -Raw
}
$coverage = if ($coverageContent) { [regex]::Match($coverageContent, 'Line coverage:\*\* \| ([\d.]+)%').Groups[1].Value } else { "0" }

# Determine badge colors
$testColor = if ([int]$failedTests -eq 0 -and [int]$totalTests -gt 0) { "brightgreen" } else { "red" }
$testStatus = if ([int]$failedTests -eq 0 -and [int]$totalTests -gt 0) { "$totalTests passing" } else { "failing" }

$coverageNum = [double]$coverage
$coverageColor = if ($coverageNum -ge 80) { "brightgreen" } elseif ($coverageNum -ge 60) { "yellow" } else { "red" }

# Update README badges
Write-Host "📝 Updating README badges..." -ForegroundColor Yellow
$readmeContent = Get-Content "readme.md" -Raw
$readmeContent = $readmeContent -replace '\!\[Tests\]\(https://img\.shields\.io/badge/tests-[^)]*\)', "![Tests](https://img.shields.io/badge/tests-$($testStatus.Replace(' ', '%20'))-$testColor)"
$readmeContent = $readmeContent -replace '\!\[Coverage\]\(https://img\.shields\.io/badge/coverage-[^)]*\)', "![Coverage](https://img.shields.io/badge/coverage-$coverage%25-$coverageColor)"
Set-Content "readme.md" -Value $readmeContent -Encoding UTF8

Write-Host ""
Write-Host "✅ Pipeline completed successfully!" -ForegroundColor Green
Write-Host "📊 Results Summary:" -ForegroundColor Green
Write-Host "   Tests: $totalTests total, $passedTests passed, $failedTests failed" -ForegroundColor White
Write-Host "   Coverage: $coverage%" -ForegroundColor White
Write-Host "   Reports: ./docs/test-results.md" -ForegroundColor White
Write-Host "   Coverage: ./TestResults/CoverageReport/index.html" -ForegroundColor White
Write-Host ""
