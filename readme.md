# LoadSharp

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

---

## 📂 Folder Structure

```
LoadSharp/
├── Program.cs                   # Main entry point
├── Core/
│   ├── LoadStep.cs             # Load step definition
│   ├── Scenario.cs             # Scenario with steps and configuration
│   ├── LoadRunner.cs           # Execution engine
│   ├── StepContext.cs          # Execution context for steps
│   ├── StepResult.cs           # Step execution result
│   └── MetricsCollector.cs     # Tracks performance metrics
├── Models/
│   ├── LoadTestConfig.cs       # Global test configuration
│   └── ScenarioStats.cs        # Runtime statistics
├── Utils/
│   └── ConsoleReporter.cs      # Simple CLI output helper
├── LoadSharp.csproj            # .NET 9 project file
└── README.md                   # You are here
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
git clone https://github.com/your-username/loadsharp.git
cd loadsharp
dotnet run
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

## 🧩 Roadmap

| Feature                  | Status    | Priority |
|--------------------------|-----------|----------|
| Step-level timing        | ✅ Done    | High     |
| Scenario weights/distribution | 🔜 Planned | High     |
| Data feeders (CSV/JSON)  | 🔜 Planned | High     |
| Assertion engine         | 🔜 Planned | Medium   |
| Ramp-up/ramp-down phases | 🔜 Planned | Medium   |
| HTML/JSON reports        | 🔜 Planned | Medium   |
| WebSocket support        | 🔜 Planned | Low      |
| Plugin architecture      | 🔜 Planned | Low      |
| Distributed testing      | 🔜 Planned | Low      |

---

## 🔒 License

MIT — use it freely in personal, academic, or commercial projects.

```
MIT License

Copyright (c) 2025 LoadSharp Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🤝 Contributing

Pull requests, feature suggestions, and bug reports are welcome. Please open an issue first to discuss major changes.

---

## 🛠 Built With

- .NET 9
- System.Diagnostics
- System.Net.Http
- System.Text.Json
- Good ol' async/await

---

## 📣 Credits

This project was inspired by modern load testing frameworks with the goal of delivering an MIT-friendly, .NET-native alternative for controlled load testing in professional environments.
