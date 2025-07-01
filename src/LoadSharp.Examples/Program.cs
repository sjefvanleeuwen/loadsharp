using LoadSharp.Core;
using LoadSharp.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

Console.WriteLine("LoadSharp Examples");
Console.WriteLine("==================");
Console.WriteLine();

// Basic HTTP scenario
var basicScenario = new Scenario("BasicHttpTest")
    .WithStep("GET /status", async ctx =>
    {
        var response = await ctx.HttpClient.GetAsync("https://httpbin.org/status/200");
        ctx.Data["statusCode"] = response.StatusCode;
        return response.IsSuccessStatusCode;
    })
    .WithStep("POST /data", async ctx =>
    {
        var content = new StringContent("{\"test\": true}", Encoding.UTF8, "application/json");
        var response = await ctx.HttpClient.PostAsync("https://httpbin.org/post", content);
        return response.IsSuccessStatusCode;
    })
    .WithDuration(TimeSpan.FromSeconds(30))
    .WithVirtualUsers(5);

// Configuration
var config = new LoadTestConfig()
    .WithWarmupDuration(TimeSpan.FromSeconds(2))
    .WithReportingInterval(TimeSpan.FromSeconds(5))
    .WithMaxFailureRate(0.1);

Console.WriteLine("Running basic HTTP load test...");
await LoadRunner.RunAsync(basicScenario, config);

Console.WriteLine();
Console.WriteLine("Press any key to run advanced example...");
Console.ReadKey();
Console.WriteLine();
Console.WriteLine();

// Advanced example with data passing
var advancedScenario = new Scenario("UserJourney")
    .WithStep("Get Random User", async ctx =>
    {
        var response = await ctx.HttpClient.GetAsync("https://httpbin.org/uuid");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);
            ctx.Data["userId"] = json.GetProperty("uuid").GetString();
            return true;
        }
        return false;
    })
    .WithStep("Use User Data", async ctx =>
    {
        var userId = ctx.Data["userId"]?.ToString();
        var requestData = new { userId, timestamp = DateTime.UtcNow };
        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await ctx.HttpClient.PostAsync("https://httpbin.org/post", content);
        return response.IsSuccessStatusCode;
    })
    .WithDuration(TimeSpan.FromSeconds(20))
    .WithVirtualUsers(3);

Console.WriteLine("Running advanced scenario with data passing...");
await LoadRunner.RunAsync(advancedScenario, config);

Console.WriteLine();
Console.WriteLine("🎉 All examples completed!");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
