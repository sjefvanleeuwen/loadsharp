using System;

namespace LoadSharp.Models;

/// <summary>
/// Global configuration for load testing.
/// </summary>
public class LoadTestConfig
{
    public TimeSpan WarmupDuration { get; private set; } = TimeSpan.Zero;
    public TimeSpan ReportingInterval { get; private set; } = TimeSpan.FromSeconds(10);
    public double MaxFailureRate { get; private set; } = 1.0; // 100% = no limit
    public TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(30);

    public LoadTestConfig WithWarmupDuration(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
            throw new ArgumentException("Warmup duration cannot be negative", nameof(duration));
        
        WarmupDuration = duration;
        return this;
    }

    public LoadTestConfig WithReportingInterval(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
            throw new ArgumentException("Reporting interval must be positive", nameof(interval));
        
        ReportingInterval = interval;
        return this;
    }

    public LoadTestConfig WithMaxFailureRate(double rate)
    {
        if (rate < 0.0 || rate > 1.0)
            throw new ArgumentException("Max failure rate must be between 0.0 and 1.0", nameof(rate));
        
        MaxFailureRate = rate;
        return this;
    }

    public LoadTestConfig WithTimeout(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentException("Timeout must be positive", nameof(timeout));
        
        Timeout = timeout;
        return this;
    }
}
