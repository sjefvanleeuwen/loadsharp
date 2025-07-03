#!/usr/bin/env pwsh
# 🔍 LoadSharp Pipeline Validation Script
# This script validates that all pipeline components are working correctly

Write-Host "🔍 LoadSharp GitOps Pipeline Validation" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

$errors = @()
$warnings = @()

# Test 1: Solution builds successfully
Write-Host "🏗️  Testing solution build..." -ForegroundColor Yellow
try {
    $buildResult = & dotnet build LoadSharp.sln --verbosity minimal --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Solution builds successfully" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Solution build failed" -ForegroundColor Red
        $errors += "Solution build failed"
    }
} catch {
    Write-Host "   ❌ Build error: $_" -ForegroundColor Red
    $errors += "Build error: $_"
}

# Test 2: All tests pass
Write-Host "🧪 Testing test execution..." -ForegroundColor Yellow
try {
    $testResult = & dotnet test LoadSharp.sln --verbosity minimal --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ All tests pass" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Some tests failed" -ForegroundColor Red
        $errors += "Test failures detected"
    }
} catch {
    Write-Host "   ❌ Test error: $_" -ForegroundColor Red
    $errors += "Test error: $_"
}

# Test 3: Workflow files are valid YAML
Write-Host "📄 Validating GitHub Actions workflows..." -ForegroundColor Yellow
$workflowFiles = Get-ChildItem ".github/workflows/*.yml" -ErrorAction SilentlyContinue
if ($workflowFiles) {
    foreach ($file in $workflowFiles) {
        try {
            # Basic YAML validation - check for common syntax issues
            $content = Get-Content $file.FullName -Raw
            if ($content -match "^\s*-\s+" -and $content -match ":\s*$") {
                Write-Host "   ✅ $($file.Name) - Valid structure" -ForegroundColor Green
            } else {
                Write-Host "   ⚠️  $($file.Name) - Check syntax" -ForegroundColor Yellow
                $warnings += "$($file.Name) may have syntax issues"
            }
        } catch {
            Write-Host "   ❌ $($file.Name) - Error reading file" -ForegroundColor Red
            $errors += "Cannot read workflow file: $($file.Name)"
        }
    }
} else {
    Write-Host "   ❌ No workflow files found" -ForegroundColor Red
    $errors += "No GitHub Actions workflows found"
}

# Test 4: Test report generator works
Write-Host "📊 Testing test report generator..." -ForegroundColor Yellow
try {
    $buildResult = & dotnet build tools/TestReportGenerator/TestReportGenerator.csproj --verbosity minimal --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✅ Test report generator builds" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Test report generator build failed" -ForegroundColor Red
        $errors += "Test report generator build failed"
    }
} catch {
    Write-Host "   ❌ Error building test report generator: $_" -ForegroundColor Red
    $errors += "Test report generator error: $_"
}

# Test 5: Required files exist
Write-Host "📁 Checking required files..." -ForegroundColor Yellow
$requiredFiles = @(
    "LoadSharp.sln",
    "src/LoadSharp/LoadSharp.csproj",
    "tests/LoadSharp.Tests/LoadSharp.Tests.csproj",
    ".github/workflows/build-and-test.yml",
    ".github/workflows/quality-gate.yml",
    ".github/workflows/release.yml",
    ".github/workflows/documentation.yml"
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "   ✅ $file" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $file - Missing" -ForegroundColor Red
        $errors += "Missing required file: $file"
    }
}

# Summary
Write-Host ""
Write-Host "📋 Validation Summary" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

if ($errors.Count -eq 0) {
    Write-Host "🎉 All validations passed! Pipeline is ready for deployment." -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor White    Write-Host "1. Push code to GitHub repository" -ForegroundColor Gray
    Write-Host "2. Configure repository Actions permissions" -ForegroundColor Gray
    Write-Host "3. Add NUGET_API_KEY secret" -ForegroundColor Gray
} else {
    Write-Host "❌ Validation failed. Please fix the following issues:" -ForegroundColor Red
    foreach ($error in $errors) {
        Write-Host "   • $error" -ForegroundColor Red
    }
}

if ($warnings.Count -gt 0) {
    Write-Host ""
    Write-Host "⚠️  Warnings:" -ForegroundColor Yellow
    foreach ($warning in $warnings) {
        Write-Host "   • $warning" -ForegroundColor Yellow
    }
}

Write-Host ""
if ($errors.Count -eq 0) {
    exit 0
} else {
    exit 1
}
