using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadSharp.Core;

/// <summary>
/// Represents a load testing scenario with steps and configuration.
/// </summary>
public class Scenario
{
    private readonly List<LoadStep> _steps = new();
    
    public string Name { get; }
    public IReadOnlyList<LoadStep> Steps => _steps.AsReadOnly();
    public TimeSpan Duration { get; private set; } = TimeSpan.FromMinutes(1);
    public int VirtualUsers { get; private set; } = 1;
    public int Weight { get; private set; } = 100;

    public Scenario(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Scenario WithStep(string stepName, Func<StepContext, Task<bool>> stepFunc)
    {
        _steps.Add(new LoadStep(stepName, stepFunc));
        return this;
    }

    public Scenario WithDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive", nameof(duration));
        
        Duration = duration;
        return this;
    }

    public Scenario WithVirtualUsers(int users)
    {
        if (users <= 0)
            throw new ArgumentException("Virtual users must be positive", nameof(users));
        
        VirtualUsers = users;
        return this;
    }

    public Scenario WithWeight(int weight)
    {
        if (weight <= 0)
            throw new ArgumentException("Weight must be positive", nameof(weight));
        
        Weight = weight;
        return this;
    }
}
