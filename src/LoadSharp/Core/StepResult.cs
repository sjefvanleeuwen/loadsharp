using System;

namespace LoadSharp.Core;

/// <summary>
/// Result of executing a load test step.
/// </summary>
public class StepResult
{
    public string StepName { get; }
    public bool Success { get; }
    public TimeSpan Duration { get; }
    public Exception? Error { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public StepResult(string stepName, bool success, TimeSpan duration, 
        DateTime startTime, DateTime endTime, Exception? error = null)
    {
        StepName = stepName ?? throw new ArgumentNullException(nameof(stepName));
        Success = success;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        Error = error;
    }

    public static StepResult CreateSuccess(string stepName, TimeSpan duration, DateTime startTime, DateTime endTime)
        => new(stepName, true, duration, startTime, endTime);

    public static StepResult CreateFailure(string stepName, TimeSpan duration, DateTime startTime, DateTime endTime, Exception error)
        => new(stepName, false, duration, startTime, endTime, error);
}
