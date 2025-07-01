@echo off
echo 🚀 LoadSharp Test Pipeline
echo =========================
echo.

REM Clean previous results
echo 🧹 Cleaning previous test results...
if exist "TestResults" rmdir /s /q "TestResults"

echo 📦 Restoring dependencies...
dotnet restore LoadSharp.sln
if %ERRORLEVEL% neq 0 (
    echo ❌ Failed to restore dependencies
    exit /b 1
)

echo 🔨 Building solution...
dotnet build LoadSharp.sln --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ❌ Failed to build solution
    exit /b 1
)

echo 🧪 Running tests with coverage...
dotnet test LoadSharp.sln --configuration Release --no-build --verbosity normal --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage" --results-directory ./TestResults
if %ERRORLEVEL% neq 0 (
    echo ❌ Tests failed
    exit /b 1
)

echo 📊 Generating coverage report...
reportgenerator -reports:"./TestResults/*/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:"Html;MarkdownSummaryGithub;Cobertura" -verbosity:Info
if %ERRORLEVEL% neq 0 (
    echo ⚠️  ReportGenerator failed - installing tool...
    dotnet tool install -g dotnet-reportgenerator-globaltool
    reportgenerator -reports:"./TestResults/*/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:"Html;MarkdownSummaryGithub;Cobertura" -verbosity:Info
)

echo 📋 Generating test report...
dotnet run --project tools/TestReportGenerator/TestReportGenerator.csproj --configuration Release -- --input "./TestResults/test-results.trx" --output "./docs/test-results.md" --coverage "./TestResults/CoverageReport/SummaryGithub.md"
if %ERRORLEVEL% neq 0 (
    echo ❌ Failed to generate test report
    exit /b 1
)

echo.
echo ✅ Pipeline completed successfully!
echo 📊 Check results:
echo    Test Report: ./docs/test-results.md  
echo    Coverage: ./TestResults/CoverageReport/index.html
echo.
