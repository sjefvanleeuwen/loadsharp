using System;
using System.Linq;
using LoadSharp.Core;
using LoadSharp.Models;

namespace LoadSharp.Utils;

/// <summary>
/// Simple console output helper for load test reporting.
/// </summary>
public class ConsoleReporter
{
    public void ReportTestStart(Scenario scenario, LoadTestConfig config)
    {
        Console.WriteLine("🚀 LoadSharp - Starting Load Test");
        Console.WriteLine($"   Scenario: {scenario.Name}");
        Console.WriteLine($"   Virtual Users: {scenario.VirtualUsers}");
        Console.WriteLine($"   Duration: {scenario.Duration}");
        Console.WriteLine($"   Steps: {scenario.Steps.Count}");
        
        if (config.WarmupDuration > TimeSpan.Zero)
        {
            Console.WriteLine($"   Warmup: {config.WarmupDuration}");
        }
        
        Console.WriteLine();
    }

    public void ReportWarmupStart(TimeSpan warmupDuration)
    {
        Console.WriteLine($"🔥 Warming up for {warmupDuration}...");
        Console.WriteLine();
    }

    public void ReportCurrentStats(ScenarioStats stats)
    {
        Console.WriteLine($"📊 [{DateTime.Now:HH:mm:ss}] {stats.ScenarioName}");
        Console.WriteLine($"   Requests: {stats.TotalRequests} total, {stats.SuccessfulRequests} success, {stats.FailedRequests} failed");
        Console.WriteLine($"   Success Rate: {stats.SuccessRate:P1}");
        Console.WriteLine($"   RPS: {stats.RequestsPerSecond:F1}");
        Console.WriteLine($"   Response Times: avg={stats.AverageResponseTime:F0}ms, p95={stats.P95ResponseTime:F0}ms, p99={stats.P99ResponseTime:F0}ms");
        
        if (stats.Errors.Any())
        {
            Console.WriteLine($"   Errors: {string.Join(", ", stats.Errors.Select(e => $"{e.Key}({e.Value})"))}");
        }
        
        Console.WriteLine();
    }

    public void ReportFinalStats(ScenarioStats stats)
    {
        Console.WriteLine("✅ Load Test Completed!");
        Console.WriteLine();
        Console.WriteLine("📈 Final Results:");
        Console.WriteLine($"   Total Duration: {stats.TestDuration}");
        Console.WriteLine($"   Total Requests: {stats.TotalRequests}");
        Console.WriteLine($"   Successful: {stats.SuccessfulRequests} ({stats.SuccessRate:P1})");
        Console.WriteLine($"   Failed: {stats.FailedRequests}");
        Console.WriteLine($"   Average RPS: {stats.RequestsPerSecond:F1}");
        Console.WriteLine();
        Console.WriteLine("⏱️  Response Time Statistics:");
        Console.WriteLine($"   Min: {stats.MinResponseTime:F0}ms");
        Console.WriteLine($"   Average: {stats.AverageResponseTime:F0}ms");
        Console.WriteLine($"   Max: {stats.MaxResponseTime:F0}ms");
        Console.WriteLine($"   P50: {stats.P50ResponseTime:F0}ms");
        Console.WriteLine($"   P95: {stats.P95ResponseTime:F0}ms");
        Console.WriteLine($"   P99: {stats.P99ResponseTime:F0}ms");
        
        if (stats.Errors.Any())
        {
            Console.WriteLine();
            Console.WriteLine("❌ Error Summary:");
            foreach (var error in stats.Errors)
            {
                Console.WriteLine($"   {error.Key}: {error.Value} occurrences");
            }
        }
        
        Console.WriteLine();
    }
}
