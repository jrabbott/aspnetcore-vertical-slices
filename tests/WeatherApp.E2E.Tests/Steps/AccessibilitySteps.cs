using System.Globalization;
using System.Text;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Reqnroll;

namespace WeatherApp.E2E.Tests.Steps;

[Binding]
public sealed class AccessibilitySteps(ScenarioContext scenarioContext)
{
    private static readonly List<string> s_wcagTags =
    [
        "wcag2a",
        "wcag2aa",
        "wcag21a",
        "wcag21aa"
    ];

    private readonly ScenarioContext _scenarioContext = scenarioContext;

    private IPage Page => _scenarioContext.Get<IPage>();

    [Then("the page should have no accessibility violations")]
    public async Task ThenThePageShouldHaveNoAccessibilityViolations()
    {
        var options = new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = s_wcagTags
            },
            ResultTypes =
            [
                ResultType.Violations
            ]
        };

        AxeResult results = await Page.RunAxe(options).ConfigureAwait(false);
        AxeResultItem[] violations = results.Violations ?? [];

        Assert.True(violations.Length == 0, FormatViolations(violations));
    }

    private static string FormatViolations(AxeResultItem[] violations)
    {
        var builder = new StringBuilder();
        builder.AppendLine(CultureInfo.InvariantCulture, $"Found {violations.Length} accessibility violation(s):");

        foreach (AxeResultItem violation in violations)
        {
            builder.AppendLine(CultureInfo.InvariantCulture, $"- [{violation.Impact}] {violation.Id}: {violation.Help}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"  {violation.HelpUrl}");

            foreach (AxeResultNode node in violation.Nodes ?? [])
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"  target: {node.Target}");
                if (!string.IsNullOrWhiteSpace(node.Html))
                {
                    builder.AppendLine(CultureInfo.InvariantCulture, $"  html: {Truncate(node.Html, 200)}");
                }
            }
        }

        return builder.ToString();
    }

    private static string Truncate(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : string.Concat(value.AsSpan(0, maxLength), "…");
    }
}
