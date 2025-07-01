using System;
using System.Collections.Generic;

namespace LoadSharp.Models;

/// <summary>
/// Runtime statistics for a load testing scenario.
/// </summary>
public class ScenarioStats
{
    public string ScenarioName { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double SuccessRate { get; set; }
    public double RequestsPerSecond { get; set; }
    public double AverageResponseTime { get; set; }
    public double MinResponseTime { get; set; }
    public double MaxResponseTime { get; set; }
    public double P50ResponseTime { get; set; }
    public double P95ResponseTime { get; set; }
    public double P99ResponseTime { get; set; }
    public TimeSpan TestDuration { get; set; }
    public Dictionary<string, int> Errors { get; set; } = new();
}
