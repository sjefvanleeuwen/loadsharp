using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LoadSharp.Models;
using LoadSharp.Utils;

namespace LoadSharp.Core;

/// <summary>
/// Main execution engine for running load tests.
/// </summary>
public static class LoadRunner
{
    public static async Task RunAsync(Scenario scenario, LoadTestConfig? config = null)
    {
        config ??= new LoadTestConfig();
        var metricsCollector = new MetricsCollector();
        var reporter = new ConsoleReporter();
        
        using var cts = new CancellationTokenSource();
        
        // Schedule test duration
        cts.CancelAfter(scenario.Duration);
        
        reporter.ReportTestStart(scenario, config);
        metricsCollector.StartTest();
        
        // Warmup phase
        if (config.WarmupDuration > TimeSpan.Zero)
        {
            reporter.ReportWarmupStart(config.WarmupDuration);
            await Task.Delay(config.WarmupDuration, cts.Token);
        }
        
        // Start virtual users
        var tasks = new List<Task>();
        for (int i = 0; i < scenario.VirtualUsers; i++)
        {
            var userId = i;
            tasks.Add(RunVirtualUserAsync(scenario, userId, metricsCollector, cts.Token));
        }
        
        // Start reporting
        var reportingTask = StartReportingAsync(scenario, metricsCollector, reporter, config.ReportingInterval, cts.Token);
        
        // Wait for completion
        await Task.WhenAll(tasks.Concat(new[] { reportingTask }));
        
        // Final report
        var finalStats = metricsCollector.GetCurrentStats(scenario.Name);
        reporter.ReportFinalStats(finalStats);
    }
    
    private static async Task RunVirtualUserAsync(Scenario scenario, int userId, 
        MetricsCollector metricsCollector, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        var userData = new Dictionary<string, object?>();
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var step in scenario.Steps)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                        
                    await ExecuteStepAsync(step, httpClient, userData, userId, scenario.Name, 
                        metricsCollector, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when test duration expires
        }
    }
    
    private static async Task ExecuteStepAsync(LoadStep step, HttpClient httpClient, 
        Dictionary<string, object?> userData, int userId, string scenarioName,
        MetricsCollector metricsCollector, CancellationToken cancellationToken)
    {
        var context = new StepContext(httpClient, userData, cancellationToken, userId, scenarioName);
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var success = await step.Execute(context);
            stopwatch.Stop();
            var endTime = DateTime.UtcNow;
            
            var result = StepResult.CreateSuccess(step.Name, stopwatch.Elapsed, startTime, endTime);
            if (!success)
            {
                result = StepResult.CreateFailure(step.Name, stopwatch.Elapsed, startTime, endTime, 
                    new Exception("Step returned false"));
            }
            
            metricsCollector.RecordResult(result);
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            stopwatch.Stop();
            var endTime = DateTime.UtcNow;
            var result = StepResult.CreateFailure(step.Name, stopwatch.Elapsed, startTime, endTime, ex);
            metricsCollector.RecordResult(result);
        }
    }
    
    private static async Task StartReportingAsync(Scenario scenario, MetricsCollector metricsCollector, 
        ConsoleReporter reporter, TimeSpan reportingInterval, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(reportingInterval, cancellationToken);
                var stats = metricsCollector.GetCurrentStats(scenario.Name);
                reporter.ReportCurrentStats(stats);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when test completes
        }
    }
}
