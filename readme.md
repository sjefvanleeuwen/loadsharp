# LoadSharp

[![Build Status](https://github.com/sjefvanleeuwen/loadsharp/workflows/Build%20and%20Test/badge.svg)](https://github.com/sjefvanleeuwen/loadsharp/actions)
[![Tests](https://img.shields.io/badge/tests-passing-brightgreen)](./docs/test-results.md)
[![Coverage](https://img.shields.io/badge/coverage-%25-red)](./docs/test-results.md)
[![Quality Gate](https://img.shields.io/badge/quality%20gate-passing-brightgreen)](https://github.com/sjefvanleeuwen/loadsharp/actions)
[![NuGet](https://img.shields.io/badge/nuget-v0.1.0-blue)](https://www.nuget.org/packages/LoadSharp)
[![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)

**LoadSharp** is a minimal, MIT-licensed load testing framework written in **C#** for **.NET 9**. It aims to be a lightweight, extensible alternative to commercial or restrictive tools, with clean scenario modeling and actionable metrics — built for developers who want full control.

---

## ✨ Features (v0.1)

- ✅ Scenario-based load testing
- ✅ Define steps with async C# code
- ✅ Configure number of virtual users
- ✅ Run for a fixed duration
- ✅ Collect metrics: success/fail count, average latency, RPS
- ✅ Built-in HTTP client support
- ✅ Step context with shared data
- ✅ Clean console output with real-time stats
- ✅ Error handling and categorization
- ✅ Fully MIT-licensed — no usage restrictions
- 🆕 Multi-step scenario chaining with fluent API
- 🆕 Advanced weight configuration for load balancing
- 🆕 Comprehensive test coverage with xUnit framework
- 🆕 Automated GitOps pipeline with GitHub Actions
- 🆕 Real-time badge updates and quality gates

---

## 📂 Solution Structure

```
LoadSharp.sln                   # Visual Studio solution file
├── src/
│   ├── LoadSharp/              # Main class library (distributable as NuGet package)
│   │   ├── LoadSharp.csproj    # Library project file
│   │   ├── Core/
│   │   │   ├── LoadStep.cs     # Load step definition
│   │   │   ├── Scenario.cs     # Scenario with steps and configuration
│   │   │   ├── LoadRunner.cs   # Execution engine
│   │   │   ├── StepContext.cs  # Execution context for steps
│   │   │   ├── StepResult.cs   # Step execution result
│   │   │   └── MetricsCollector.cs # Tracks performance metrics
│   │   ├── Models/
│   │   │   ├── LoadTestConfig.cs   # Global test configuration
│   │   │   └── ScenarioStats.cs    # Runtime statistics
│   │   └── Utils/
│   │       └── ConsoleReporter.cs  # Simple CLI output helper
│   └── LoadSharp.Examples/     # Console app with usage examples
│       ├── LoadSharp.Examples.csproj
│       └── Program.cs          # Example scenarios and usage
└── tests/
    └── LoadSharp.Tests/        # xUnit test project
        ├── LoadSharp.Tests.csproj
        └── UnitTest1.cs        # Unit tests for the framework
```

---

## 🧪 Example Usage

```csharp
using LoadSharp.Core;

// Basic HTTP scenario
var scenario = new Scenario("HttpTest")
    .WithStep("GET /status", async ctx =>
    {
        var res = await ctx.HttpClient.GetAsync("https://httpbin.org/status/200");
        ctx.Data["statusCode"] = res.StatusCode;
        return res.IsSuccessStatusCode;
    })
    .WithStep("POST /data", async ctx =>
    {
        var content = new StringContent("{\"test\": true}", Encoding.UTF8, "application/json");
        var res = await ctx.HttpClient.PostAsync("https://httpbin.org/post", content);
        return res.IsSuccessStatusCode;
    })
    .WithDuration(TimeSpan.FromSeconds(30))
    .WithVirtualUsers(10);

// Configuration options
var config = new LoadTestConfig()
    .WithWarmupDuration(TimeSpan.FromSeconds(5))
    .WithReportingInterval(TimeSpan.FromSeconds(5))
    .WithMaxFailureRate(0.1); // Stop if 10% of requests fail

await LoadRunner.RunAsync(scenario, config);
```

**Advanced Example:**
```csharp
// Multi-step scenario with data passing
var scenario = new Scenario("UserJourney")
    .WithStep("Login", async ctx =>
    {
        var loginData = new { username = "test", password = "pass" };
        var content = JsonContent.Create(loginData);
        var res = await ctx.HttpClient.PostAsync("/api/login", content);
        
        if (res.IsSuccessStatusCode)
        {
            var token = await res.Content.ReadAsStringAsync();
            ctx.Data["authToken"] = token;
            return true;
        }
        return false;
    })
    .WithStep("GetProfile", async ctx =>
    {
        var token = ctx.Data["authToken"]?.ToString();
        ctx.HttpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var res = await ctx.HttpClient.GetAsync("/api/profile");
        return res.IsSuccessStatusCode;
    })
    .WithWeight(70) // 70% of virtual users run this scenario
    .WithVirtualUsers(50)
    .WithDuration(TimeSpan.FromMinutes(5));
```

---

## ⚙️ How to Run

### 🧱 Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- VSCode, Rider, or any IDE

### ▶️ Run It
```bash
git clone https://github.com/sjefvanleeuwen/loadsharp.git
cd loadsharp
dotnet run --project src/LoadSharp.Examples
```

**Or run tests:**
```bash
dotnet test
```

**Or build the library:**
```bash
dotnet build src/LoadSharp
```

---

## 📊 Metrics & Reporting

LoadSharp collects the following metrics:
- **Request Rate (RPS)** - Requests per second
- **Response Times** - Min, Max, Average, P50, P95, P99
- **Success Rate** - Percentage of successful requests
- **Error Distribution** - Categorized error types
- **Virtual User Activity** - Active users over time

---

## 🔄 CI/CD Pipeline

LoadSharp uses a comprehensive GitOps workflow with automated testing, quality gates, and releases.

### 🚀 GitHub Actions Workflows

#### Build and Test (Main)
- **Trigger:** Push to `main` or `develop`, Pull Requests to `main`
- **Features:**
  - ✅ Automated testing with xUnit
  - 📊 Code coverage analysis with ReportGenerator
  - 📋 Automated test report generation
  - 🏷️ Dynamic badge updates
  - 📦 NuGet package validation

#### Quality Gate (PR)
- **Trigger:** Pull Requests to `main`
- **Features:**
  - ✅ Code coverage threshold validation (20% minimum)
  - 📦 Package build verification
  - 💬 Automated PR comments with test results
  - 🚫 Blocks merge if quality standards not met

#### Release (Tags)
- **Trigger:** Version tags (`v*`)
- **Features:**
  - 🎯 Automatic GitHub releases
  - 📦 NuGet package publishing
  - 📄 Generated release notes
  - 📊 Test results in release artifacts

#### Documentation (API Docs)
- **Trigger:** Changes to source code or docs
- **Features:**
  - 📚 Automated API documentation with DocFX
  - 🌐 GitHub Pages deployment
  - 🔄 Live updates on code changes

### 🧪 Local Development Pipeline

#### Quick Test Run
```bash
# Windows
.\run-tests.cmd

# Cross-platform
dotnet test
```

#### Full Pipeline (with Coverage)
```powershell
# Run complete pipeline locally
.\run-tests.ps1

# Or manually step by step
dotnet restore LoadSharp.sln
dotnet build LoadSharp.sln --configuration Release --no-restore
dotnet test LoadSharp.sln --configuration Release --no-build --collect:"XPlat Code Coverage"
reportgenerator -reports:"./TestResults/*/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:"Html;MarkdownSummaryGithub"
```

### 📊 Test Reporting

The pipeline automatically generates:
- **Test Results:** [./docs/test-results.md](./docs/test-results.md)
- **Coverage Report:** `./TestResults/CoverageReport/index.html`
- **TRX Files:** Machine-readable test results
- **Coverage XML:** Cobertura format for external tools

### 🏷️ Badge System

Badges are automatically updated after each successful build:
- **Tests:** Shows current test count and status
- **Coverage:** Real-time code coverage percentage
- **Build:** GitHub Actions build status
- **Version:** Current NuGet package version

### 🔄 Release Process

1. **Development:** Work on feature branches
2. **Pull Request:** Quality gate runs automatically
3. **Merge:** Main pipeline updates badges and reports
4. **Tag Release:** `git tag v1.0.1 && git push origin v1.0.1`
5. **Automatic:** GitHub release + NuGet publish

---

## 🧑‍💻 Contributing

### Development Workflow
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and add tests
4. Run the local pipeline: `.\run-tests.cmd`
5. Commit your changes: `git commit -m 'Add amazing feature'`
6. Push to the branch: `git push origin feature/amazing-feature`
7. Open a Pull Request

### Quality Standards
- ✅ All tests must pass
- 📊 Code coverage should not decrease
- 📝 Public APIs must be documented
- 🧪 New features should include tests
- 📋 Follow existing code style

### Local Setup
```bash
# Clone the repository
git clone https://github.com/sjefvanleeuwen/loadsharp.git
cd loadsharp

# Restore dependencies
dotnet restore

# Run tests
.\run-tests.cmd

# Run examples
dotnet run --project src/LoadSharp.Examples
```

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 🤝 Support

- 🐛 **Issues:** [GitHub Issues](https://github.com/sjefvanleeuwen/loadsharp/issues)
- 💬 **Discussions:** [GitHub Discussions](https://github.com/sjefvanleeuwen/loadsharp/discussions)
- 📖 **Documentation:** [GitHub Pages](https://sjefvanleeuwen.github.io/loadsharp/)

---

**Built with ❤️ for the .NET community**
