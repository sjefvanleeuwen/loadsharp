using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace LoadSharp.Core;

/// <summary>
/// Execution context for a load test step, providing shared data and HTTP client.
/// </summary>
public class StepContext
{
    public HttpClient HttpClient { get; }
    public Dictionary<string, object?> Data { get; }
    public CancellationToken CancellationToken { get; }
    public int VirtualUserId { get; }
    public string ScenarioName { get; }

    public StepContext(HttpClient httpClient, Dictionary<string, object?> data, 
        CancellationToken cancellationToken, int virtualUserId, string scenarioName)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        CancellationToken = cancellationToken;
        VirtualUserId = virtualUserId;
        ScenarioName = scenarioName ?? throw new ArgumentNullException(nameof(scenarioName));
    }
}
