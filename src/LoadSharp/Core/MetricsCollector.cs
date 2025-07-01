using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoadSharp.Models;

namespace LoadSharp.Core;

/// <summary>
/// Collects and aggregates performance metrics during load testing.
/// </summary>
public class MetricsCollector
{
    private readonly object _lock = new();
    private readonly List<StepResult> _results = new();
    private DateTime _testStartTime;
    private DateTime _lastReportTime;

    public void StartTest()
    {
        lock (_lock)
        {
            _testStartTime = DateTime.UtcNow;
            _lastReportTime = _testStartTime;
            _results.Clear();
        }
    }

    public void RecordResult(StepResult result)
    {
        lock (_lock)
        {
            _results.Add(result);
        }
    }

    public ScenarioStats GetCurrentStats(string scenarioName)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var totalDuration = now - _testStartTime;
            var timeSinceLastReport = now - _lastReportTime;

            var successfulResults = _results.Where(r => r.Success).ToList();
            var failedResults = _results.Where(r => !r.Success).ToList();

            var totalRequests = _results.Count;
            var successfulRequests = successfulResults.Count;
            var failedRequests = failedResults.Count;

            var successRate = totalRequests > 0 ? (double)successfulRequests / totalRequests : 0.0;
            var rps = totalDuration.TotalSeconds > 0 ? totalRequests / totalDuration.TotalSeconds : 0.0;

            var responseTimes = successfulResults.Select(r => r.Duration.TotalMilliseconds).ToList();
            responseTimes.Sort();

            var stats = new ScenarioStats
            {
                ScenarioName = scenarioName,
                TotalRequests = totalRequests,
                SuccessfulRequests = successfulRequests,
                FailedRequests = failedRequests,
                SuccessRate = successRate,
                RequestsPerSecond = rps,
                AverageResponseTime = responseTimes.Count > 0 ? responseTimes.Average() : 0.0,
                MinResponseTime = responseTimes.Count > 0 ? responseTimes.Min() : 0.0,
                MaxResponseTime = responseTimes.Count > 0 ? responseTimes.Max() : 0.0,
                P50ResponseTime = GetPercentile(responseTimes, 0.50),
                P95ResponseTime = GetPercentile(responseTimes, 0.95),
                P99ResponseTime = GetPercentile(responseTimes, 0.99),
                TestDuration = totalDuration,
                Errors = failedResults.GroupBy(r => r.Error?.GetType().Name ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            _lastReportTime = now;
            return stats;
        }
    }

    private static double GetPercentile(List<double> sortedValues, double percentile)
    {
        if (sortedValues.Count == 0) return 0.0;
        
        var index = (int)Math.Ceiling(sortedValues.Count * percentile) - 1;
        index = Math.Max(0, Math.Min(sortedValues.Count - 1, index));
        return sortedValues[index];
    }
}
