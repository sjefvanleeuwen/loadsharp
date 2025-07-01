using LoadSharp.Core;
using LoadSharp.Models;

namespace LoadSharp.Tests;

public class ScenarioTests
{
    [Fact]
    public void Scenario_Constructor_SetsName()
    {
        // Arrange & Act
        var scenario = new Scenario("TestScenario");

        // Assert
        Assert.Equal("TestScenario", scenario.Name);
        Assert.Empty(scenario.Steps);
        Assert.Equal(TimeSpan.FromMinutes(1), scenario.Duration);
        Assert.Equal(1, scenario.VirtualUsers);
        Assert.Equal(100, scenario.Weight);
    }

    [Fact]
    public void Scenario_WithStep_AddsStep()
    {
        // Arrange
        var scenario = new Scenario("TestScenario");

        // Act
        scenario.WithStep("Test Step", async ctx => true);

        // Assert
        Assert.Single(scenario.Steps);
        Assert.Equal("Test Step", scenario.Steps[0].Name);
    }

    [Fact]
    public void Scenario_WithDuration_SetsDuration()
    {
        // Arrange
        var scenario = new Scenario("TestScenario");
        var duration = TimeSpan.FromMinutes(5);

        // Act
        scenario.WithDuration(duration);

        // Assert
        Assert.Equal(duration, scenario.Duration);
    }

    [Fact]
    public void Scenario_WithVirtualUsers_SetsVirtualUsers()
    {
        // Arrange
        var scenario = new Scenario("TestScenario");

        // Act
        scenario.WithVirtualUsers(10);

        // Assert
        Assert.Equal(10, scenario.VirtualUsers);
    }

    [Fact]
    public void Scenario_WithWeight_SetsWeight()
    {
        // Arrange
        var scenario = new Scenario("TestScenario");

        // Act
        scenario.WithWeight(75);

        // Assert
        Assert.Equal(75, scenario.Weight);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Scenario_WithVirtualUsers_ThrowsForInvalidValues(int users)
    {
        // Arrange
        var scenario = new Scenario("TestScenario");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => scenario.WithVirtualUsers(users));
    }

    [Fact]
    public void Scenario_WithMultipleSteps_AddsAllSteps()
    {
        // Arrange
        var scenario = new Scenario("MultiStepScenario");

        // Act
        scenario
            .WithStep("Step 1", async ctx => true)
            .WithStep("Step 2", async ctx => true)
            .WithStep("Step 3", async ctx => false);

        // Assert
        Assert.Equal(3, scenario.Steps.Count);
        Assert.Equal("Step 1", scenario.Steps[0].Name);
        Assert.Equal("Step 2", scenario.Steps[1].Name);
        Assert.Equal("Step 3", scenario.Steps[2].Name);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(150)]
    [InlineData(200)]
    public void Scenario_WithWeight_AcceptsValidValues(int weight)
    {
        // Arrange
        var scenario = new Scenario("TestScenario");

        // Act
        scenario.WithWeight(weight);

        // Assert
        Assert.Equal(weight, scenario.Weight);
    }
}

public class LoadTestConfigTests
{
    [Fact]
    public void LoadTestConfig_Constructor_SetsDefaults()
    {
        // Arrange & Act
        var config = new LoadTestConfig();

        // Assert
        Assert.Equal(TimeSpan.Zero, config.WarmupDuration);
        Assert.Equal(TimeSpan.FromSeconds(10), config.ReportingInterval);
        Assert.Equal(1.0, config.MaxFailureRate);
        Assert.Equal(TimeSpan.FromSeconds(30), config.Timeout);
    }

    [Fact]
    public void LoadTestConfig_WithWarmupDuration_SetsValue()
    {
        // Arrange
        var config = new LoadTestConfig();
        var duration = TimeSpan.FromSeconds(5);

        // Act
        config.WithWarmupDuration(duration);

        // Assert
        Assert.Equal(duration, config.WarmupDuration);
    }

    [Fact]
    public void LoadTestConfig_WithMaxFailureRate_SetsValue()
    {
        // Arrange
        var config = new LoadTestConfig();

        // Act
        config.WithMaxFailureRate(0.1);

        // Assert
        Assert.Equal(0.1, config.MaxFailureRate);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void LoadTestConfig_WithMaxFailureRate_ThrowsForInvalidValues(double rate)
    {
        // Arrange
        var config = new LoadTestConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithMaxFailureRate(rate));
    }
}

public class StepResultTests
{
    [Fact]
    public void StepResult_CreateSuccess_CreatesSuccessfulResult()
    {
        // Arrange
        var stepName = "Test Step";
        var duration = TimeSpan.FromMilliseconds(100);
        var startTime = DateTime.UtcNow;
        var endTime = startTime.Add(duration);

        // Act
        var result = StepResult.CreateSuccess(stepName, duration, startTime, endTime);

        // Assert
        Assert.Equal(stepName, result.StepName);
        Assert.True(result.Success);
        Assert.Equal(duration, result.Duration);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Null(result.Error);
    }

    [Fact]
    public void StepResult_CreateFailure_CreatesFailedResult()
    {
        // Arrange
        var stepName = "Test Step";
        var duration = TimeSpan.FromMilliseconds(100);
        var startTime = DateTime.UtcNow;
        var endTime = startTime.Add(duration);
        var error = new Exception("Test error");

        // Act
        var result = StepResult.CreateFailure(stepName, duration, startTime, endTime, error);

        // Assert
        Assert.Equal(stepName, result.StepName);
        Assert.False(result.Success);
        Assert.Equal(duration, result.Duration);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal(error, result.Error);
    }
}
