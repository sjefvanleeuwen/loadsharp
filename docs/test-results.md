# 🧪 Test Results Report

**Generated:** 2025-07-03 13:25:34 UTC
**Test Run:** 2025-07-03 13:25:34 - 2025-07-03 13:25:34
**Duration:** 00:00:00
**Run by:** Unknown

## 📊 Summary

| Metric | Value |
|--------|-------|
| **Total Tests** | 18 |
| **Executed** | 18 |
| **✅ Passed** | 18 |
| **❌ Failed** | 0 |
| **⚠️ Error** | 0 |
| **⏱️ Timeout** | 0 |
| **🚫 Aborted** | 0 |
| **❓ Inconclusive** | 0 |
| **Success Rate** | 100.0% |

## ✅ Overall Status

🎉 **All tests passed successfully!**

## 📈 Code Coverage

### Detailed Coverage Report

# Summary
<details open><summary>Summary</summary>

|||
|:---|:---|
| Generated on: | 07/03/2025 - 13:25:31 |
| Coverage date: | 07/03/2025 - 13:25:29 |
| Parser: | Cobertura |
| Assemblies: | 1 |
| Classes: | 9 |
| Files: | 9 |
| **Line coverage:** | 22.1% (54 of 244) |
| Covered lines: | 54 |
| Uncovered lines: | 190 |
| Coverable lines: | 244 |
| Total lines: | 519 |
| **Branch coverage:** | 18.5% (13 of 70) |
| Covered branches: | 13 |
| Total branches: | 70 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

</details>

## Coverage
<details><summary>LoadSharp - 22.1%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**LoadSharp**|**22.1%**|**18.5%**|
|LoadSharp.Core.LoadRunner|0%|0%|
|LoadSharp.Core.LoadStep|83.3%|50%|
|LoadSharp.Core.MetricsCollector|0%|0%|
|LoadSharp.Core.Scenario|91.3%|62.5%|
|LoadSharp.Core.StepContext|0%|0%|
|LoadSharp.Core.StepResult|100%|50%|
|LoadSharp.Models.LoadTestConfig|55%|50%|
|LoadSharp.Models.ScenarioStats|0%||
|LoadSharp.Utils.ConsoleReporter|0%|0%|

</details>

## ⏱️ Test Execution Details

| Test Name | Outcome | Duration |
|-----------|---------|----------|
| LoadSharp.Tests.ScenarioTests.Scenario_WithVirtualUsers_ThrowsForInvalidValues(users: -1) | ✅ Passed | 00:00:00.0002019 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithWeight_AcceptsValidValues(weight: 150) | ✅ Passed | 00:00:00.0000088 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithVirtualUsers_SetsVirtualUsers | ✅ Passed | 00:00:00.0002010 |
| LoadSharp.Tests.LoadTestConfigTests.LoadTestConfig_WithMaxFailureRate_SetsValue | ✅ Passed | 00:00:00.0001767 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithWeight_AcceptsValidValues(weight: 50) | ✅ Passed | 00:00:00.0002932 |
| LoadSharp.Tests.StepResultTests.StepResult_CreateSuccess_CreatesSuccessfulResult | ✅ Passed | 00:00:00.0032123 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithVirtualUsers_ThrowsForInvalidValues(users: 0) | ✅ Passed | 00:00:00.0009138 |
| LoadSharp.Tests.LoadTestConfigTests.LoadTestConfig_WithMaxFailureRate_ThrowsForInvalidValues(rate: 1.1000000000000001) | ✅ Passed | 00:00:00.0001818 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithStep_AddsStep | ✅ Passed | 00:00:00.0099652 |
| LoadSharp.Tests.LoadTestConfigTests.LoadTestConfig_WithMaxFailureRate_ThrowsForInvalidValues(rate: -0.10000000000000001) | ✅ Passed | 00:00:00.0022741 |
| LoadSharp.Tests.LoadTestConfigTests.LoadTestConfig_WithWarmupDuration_SetsValue | ✅ Passed | 00:00:00.0100408 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithWeight_AcceptsValidValues(weight: 200) | ✅ Passed | 00:00:00.0031698 |
| LoadSharp.Tests.ScenarioTests.Scenario_Constructor_SetsName | ✅ Passed | 00:00:00.0018743 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithDuration_SetsDuration | ✅ Passed | 00:00:00.0003043 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithWeight_SetsWeight | ✅ Passed | 00:00:00.0000774 |
| LoadSharp.Tests.ScenarioTests.Scenario_WithMultipleSteps_AddsAllSteps | ✅ Passed | 00:00:00.0007061 |
| LoadSharp.Tests.StepResultTests.StepResult_CreateFailure_CreatesFailedResult | ✅ Passed | 00:00:00.0122672 |
| LoadSharp.Tests.LoadTestConfigTests.LoadTestConfig_Constructor_SetsDefaults | ✅ Passed | 00:00:00.0029149 |

---
*Report generated by LoadSharp Test Report Generator at 2025-07-03 13:25:34 UTC*
