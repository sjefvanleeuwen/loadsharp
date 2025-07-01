using System;
using System.Threading.Tasks;

namespace LoadSharp.Core;

/// <summary>
/// Represents a single step in a load testing scenario.
/// </summary>
public class LoadStep
{
    public string Name { get; }
    public Func<StepContext, Task<bool>> Execute { get; }

    public LoadStep(string name, Func<StepContext, Task<bool>> execute)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Execute = execute ?? throw new ArgumentNullException(nameof(execute));
    }
}
