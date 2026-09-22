using System.Reflection;
using NetArchTest.Rules;

namespace WeatherApp.Architecture.Tests;

/// <summary>
/// Enforces Vertical Slice boundaries: each feature owns its types and
/// must not take compile-time dependencies on sibling feature slices.
/// </summary>
public sealed class FeatureBoundaryTests
{
    private static readonly Assembly WebAssembly = typeof(Program).Assembly;

    private static readonly string[] WeatherFeatureSlices =
    [
        "WeatherApp.Features.Weather.Search",
        "WeatherApp.Features.Weather.Forecast",
        "WeatherApp.Features.Weather.Favorites",
        "WeatherApp.Features.Weather.AddFavorite",
        "WeatherApp.Features.Weather.RemoveFavorite"
    ];

    public static TheoryData<string, string> FeatureSlicePairs
    {
        get
        {
            var data = new TheoryData<string, string>();

            foreach (var slice in WeatherFeatureSlices)
            {
                foreach (var other in WeatherFeatureSlices.Where(s => s != slice))
                {
                    data.Add(slice, other);
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(FeatureSlicePairs))]
    public void Feature_Slice_Should_Not_Depend_On_Sibling_Feature_Slices(string slice, string otherSlice)
    {
        var result = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespace(slice)
            .ShouldNot()
            .HaveDependencyOn(otherSlice)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"Types in '{slice}' must not depend on '{otherSlice}'. {FormatFailures(result)}");
    }

    [Theory]
    [InlineData("WeatherApp.Features.Weather.Search", "Search")]
    [InlineData("WeatherApp.Features.Weather.Forecast", "Forecast")]
    [InlineData("WeatherApp.Features.Weather.Favorites", "Favorites")]
    [InlineData("WeatherApp.Features.Weather.AddFavorite", "AddFavorite")]
    [InlineData("WeatherApp.Features.Weather.RemoveFavorite", "RemoveFavorite")]
    public void Feature_Slice_Should_Own_Its_Controller_Request_And_Handler(string sliceNamespace, string sliceName)
    {
        var types = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespace(sliceNamespace)
            .GetTypes()
            .Select(t => t.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains($"{sliceName}Controller", types);
        Assert.Contains($"{sliceName}Request", types);
        Assert.Contains($"{sliceName}Handler", types);
    }

    [Theory]
    [InlineData("WeatherApp.Features.Weather.Search", "Search")]
    [InlineData("WeatherApp.Features.Weather.Forecast", "Forecast")]
    [InlineData("WeatherApp.Features.Weather.AddFavorite", "AddFavorite")]
    [InlineData("WeatherApp.Features.Weather.RemoveFavorite", "RemoveFavorite")]
    public void Feature_Slice_Should_Own_Its_Request_Validator(string sliceNamespace, string sliceName)
    {
        var types = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespace(sliceNamespace)
            .GetTypes()
            .Select(t => t.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains($"{sliceName}RequestValidator", types);
    }

    [Theory]
    [InlineData("WeatherApp.Features.Weather.Search", "Search")]
    [InlineData("WeatherApp.Features.Weather.Forecast", "Forecast")]
    [InlineData("WeatherApp.Features.Weather.Favorites", "Favorites")]
    [InlineData("WeatherApp.Features.Weather.AddFavorite", "AddFavorite")]
    [InlineData("WeatherApp.Features.Weather.RemoveFavorite", "RemoveFavorite")]
    public void Feature_Slice_Should_Own_Its_Response(string sliceNamespace, string sliceName)
    {
        var types = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespace(sliceNamespace)
            .GetTypes()
            .Select(t => t.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains($"{sliceName}Response", types);
    }

    [Theory]
    [InlineData("WeatherApp.Features.Weather.Search")]
    [InlineData("WeatherApp.Features.Weather.Forecast")]
    [InlineData("WeatherApp.Features.Weather.Favorites")]
    [InlineData("WeatherApp.Features.Weather.AddFavorite")]
    [InlineData("WeatherApp.Features.Weather.RemoveFavorite")]
    public void Feature_Controller_Should_Only_Use_Handler_From_Same_Slice(string sliceNamespace)
    {
        var otherHandlers = WeatherFeatureSlices
            .Where(s => s != sliceNamespace)
            .Select(s => $"{s}.{s[(s.LastIndexOf('.') + 1)..]}Handler")
            .ToArray();

        var result = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespace(sliceNamespace)
            .And()
            .HaveNameEndingWith("Controller")
            .ShouldNot()
            .HaveDependencyOnAny(otherHandlers)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            $"Controller in '{sliceNamespace}' must not depend on handlers from other slices. {FormatFailures(result)}");
    }

    [Fact]
    public void Feature_Handlers_Should_Not_Depend_On_Other_Feature_Handlers()
    {
        var handlerTypes = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespaceStartingWith("WeatherApp.Features.Weather")
            .And()
            .HaveNameEndingWith("Handler")
            .GetTypes()
            .ToArray();

        foreach (var handlerType in handlerTypes)
        {
            var otherHandlers = handlerTypes
                .Where(t => t != handlerType)
                .Select(t => t.FullName!)
                .ToArray();

            var result = Types.InAssembly(WebAssembly)
                .That()
                .ResideInNamespace(handlerType.Namespace!)
                .And()
                .HaveName(handlerType.Name)
                .ShouldNot()
                .HaveDependencyOnAny(otherHandlers)
                .GetResult();

            Assert.True(
                result.IsSuccessful,
                $"Handler '{handlerType.FullName}' must not depend on other feature handlers. {FormatFailures(result)}");
        }
    }

    [Fact]
    public void Weather_Feature_Types_Should_Not_Depend_On_Home_Feature()
    {
        var result = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespaceStartingWith("WeatherApp.Features.Weather")
            .ShouldNot()
            .HaveDependencyOn("WeatherApp.Features.Home")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Feature_Types_Should_Not_Depend_On_Program_Or_View_Location_Expander()
    {
        var result = Types.InAssembly(WebAssembly)
            .That()
            .ResideInNamespaceStartingWith("WeatherApp.Features")
            .ShouldNot()
            .HaveDependencyOnAny(
                "WeatherApp.Program",
                "WeatherApp.Razor.FeatureViewLocationExpander")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    private static string FormatFailures(NetArchTest.Rules.TestResult result)
    {
        if (result.IsSuccessful || result.FailingTypes is null || !result.FailingTypes.Any())
        {
            return "Architecture rule failed.";
        }

        return "Failing types: " + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
    }
}
