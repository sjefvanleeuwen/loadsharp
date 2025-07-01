using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var inputOption = new Option<FileInfo>(
    name: "--input",
    description: "Path to the TRX test results file")
{ IsRequired = true };

var outputOption = new Option<FileInfo>(
    name: "--output", 
    description: "Path to the output markdown file")
{ IsRequired = true };

var coverageOption = new Option<FileInfo?>(
    name: "--coverage",
    description: "Path to the coverage summary markdown file");

var rootCommand = new RootCommand("Generate test report from TRX file")
{
    inputOption,
    outputOption,
    coverageOption
};

rootCommand.SetHandler(async (input, output, coverage) =>
{
    await GenerateTestReport(input, output, coverage);
}, inputOption, outputOption, coverageOption);

return await rootCommand.InvokeAsync(args);

static async Task GenerateTestReport(FileInfo inputFile, FileInfo outputFile, FileInfo? coverageFile)
{
    try
    {
        Console.WriteLine($"📊 Generating test report from {inputFile.Name}...");
        
        // Parse TRX file
        var trxDoc = XDocument.Load(inputFile.FullName);
        var ns = trxDoc.Root?.GetDefaultNamespace();
        
        if (ns == null)
        {
            throw new InvalidOperationException("Could not determine TRX namespace");
        }
        
        // Extract test summary
        var counters = trxDoc.Descendants(ns + "Counters").FirstOrDefault();
        var total = int.Parse(counters?.Attribute("total")?.Value ?? "0");
        var executed = int.Parse(counters?.Attribute("executed")?.Value ?? "0");
        var passed = int.Parse(counters?.Attribute("passed")?.Value ?? "0");
        var failed = int.Parse(counters?.Attribute("failed")?.Value ?? "0");
        var error = int.Parse(counters?.Attribute("error")?.Value ?? "0");
        var timeout = int.Parse(counters?.Attribute("timeout")?.Value ?? "0");
        var aborted = int.Parse(counters?.Attribute("aborted")?.Value ?? "0");
        var inconclusive = int.Parse(counters?.Attribute("inconclusive")?.Value ?? "0");
        
        var successRate = total > 0 ? (double)passed / total * 100 : 0;
        
        // Get test run info
        var testRun = trxDoc.Descendants(ns + "TestRun").FirstOrDefault();
        var runUser = testRun?.Attribute("runUser")?.Value ?? "Unknown";
        var started = DateTime.Parse(testRun?.Attribute("started")?.Value ?? DateTime.Now.ToString());
        var finished = DateTime.Parse(testRun?.Attribute("finished")?.Value ?? DateTime.Now.ToString());
        var duration = finished - started;
        
        // Extract individual test results
        var unitTestResults = trxDoc.Descendants(ns + "UnitTestResult").ToList();
        var failedTests = unitTestResults.Where(t => t.Attribute("outcome")?.Value != "Passed").ToList();
        
        // Generate markdown report
        var report = new StringBuilder();
        report.AppendLine("# 🧪 Test Results Report");
        report.AppendLine();
        report.AppendLine($"**Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        report.AppendLine($"**Test Run:** {started:yyyy-MM-dd HH:mm:ss} - {finished:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"**Duration:** {duration:hh\\:mm\\:ss}");
        report.AppendLine($"**Run by:** {runUser}");
        report.AppendLine();
        
        // Summary table
        report.AppendLine("## 📊 Summary");
        report.AppendLine();
        report.AppendLine("| Metric | Value |");
        report.AppendLine("|--------|-------|");
        report.AppendLine($"| **Total Tests** | {total} |");
        report.AppendLine($"| **Executed** | {executed} |");
        report.AppendLine($"| **✅ Passed** | {passed} |");
        report.AppendLine($"| **❌ Failed** | {failed} |");
        report.AppendLine($"| **⚠️ Error** | {error} |");
        report.AppendLine($"| **⏱️ Timeout** | {timeout} |");
        report.AppendLine($"| **🚫 Aborted** | {aborted} |");
        report.AppendLine($"| **❓ Inconclusive** | {inconclusive} |");
        report.AppendLine($"| **Success Rate** | {successRate:F1}% |");
        report.AppendLine();
        
        // Status indicators
        var statusEmoji = failed == 0 && error == 0 ? "✅" : "❌";
        report.AppendLine($"## {statusEmoji} Overall Status");
        report.AppendLine();
        if (failed == 0 && error == 0)
        {
            report.AppendLine("🎉 **All tests passed successfully!**");
        }
        else
        {
            report.AppendLine($"⚠️ **{failed + error} test(s) failed or had errors.**");
        }
        report.AppendLine();
        
        // Failed tests details
        if (failedTests.Any())
        {
            report.AppendLine("## ❌ Failed Tests");
            report.AppendLine();
              foreach (var test in failedTests)
            {
                var testName = test.Attribute("testName")?.Value ?? "Unknown";
                var outcome = test.Attribute("outcome")?.Value ?? "Unknown";
                var testDuration = test.Attribute("duration")?.Value ?? "Unknown";
                
                report.AppendLine($"### {testName}");
                report.AppendLine($"- **Outcome:** {outcome}");
                report.AppendLine($"- **Duration:** {testDuration}");
                
                // Get error message if available
                var output = test.Descendants(ns + "Output").FirstOrDefault();
                var errorInfo = output?.Descendants(ns + "ErrorInfo").FirstOrDefault();
                var message = errorInfo?.Descendants(ns + "Message").FirstOrDefault()?.Value;
                var stackTrace = errorInfo?.Descendants(ns + "StackTrace").FirstOrDefault()?.Value;
                
                if (!string.IsNullOrEmpty(message))
                {
                    report.AppendLine($"- **Error:** {message}");
                }
                
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    report.AppendLine("- **Stack Trace:**");
                    report.AppendLine("```");
                    report.AppendLine(stackTrace);
                    report.AppendLine("```");
                }
                
                report.AppendLine();
            }
        }
        
        // Code coverage section
        if (coverageFile?.Exists == true)
        {
            report.AppendLine("## 📈 Code Coverage");
            report.AppendLine();
            
            var coverageContent = await File.ReadAllTextAsync(coverageFile.FullName);
            
            // Extract coverage percentage using regex
            var coverageMatch = Regex.Match(coverageContent, @"Line coverage: ([\d.]+)%");
            if (coverageMatch.Success)
            {
                var coveragePercent = double.Parse(coverageMatch.Groups[1].Value);
                var coverageEmoji = coveragePercent >= 80 ? "🟢" : coveragePercent >= 60 ? "🟡" : "🔴";
                
                report.AppendLine($"{coverageEmoji} **Line Coverage: {coveragePercent:F1}%**");
                report.AppendLine();
            }
            
            // Include the full coverage report
            report.AppendLine("### Detailed Coverage Report");
            report.AppendLine();
            report.AppendLine(coverageContent);
        }
        
        // Test execution timeline
        report.AppendLine("## ⏱️ Test Execution Details");
        report.AppendLine();
        report.AppendLine("| Test Name | Outcome | Duration |");
        report.AppendLine("|-----------|---------|----------|");
        
        foreach (var test in unitTestResults)
        {
            var testName = test.Attribute("testName")?.Value ?? "Unknown";
            var outcome = test.Attribute("outcome")?.Value ?? "Unknown";
            var testDuration = test.Attribute("duration")?.Value ?? "Unknown";
            var outcomeEmoji = outcome == "Passed" ? "✅" : "❌";
            
            report.AppendLine($"| {testName} | {outcomeEmoji} {outcome} | {testDuration} |");
        }
        
        report.AppendLine();
        report.AppendLine("---");
        report.AppendLine($"*Report generated by LoadSharp Test Report Generator at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");
        
        // Ensure output directory exists
        outputFile.Directory?.Create();
        
        // Write the report
        await File.WriteAllTextAsync(outputFile.FullName, report.ToString());
        
        Console.WriteLine($"✅ Test report generated successfully: {outputFile.FullName}");
        Console.WriteLine($"📊 Tests: {total} total, {passed} passed, {failed} failed");
        Console.WriteLine($"📈 Success rate: {successRate:F1}%");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error generating test report: {ex.Message}");
        Environment.Exit(1);
    }
}
