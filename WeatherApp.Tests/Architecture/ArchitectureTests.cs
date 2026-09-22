using System.Reflection;
using NetArchTest.Rules;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favorites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Tests.Architecture;

public sealed class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(Location).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(IWeatherClient).Assembly;
    private static readonly Assembly WebAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_Should_Not_Reference_Infrastructure_Or_Web_Assemblies()
    {
        var references = GetReferencedAssemblyNames(DomainAssembly);

        Assert.DoesNotContain(InfrastructureAssembly.GetName().Name, references);
        Assert.DoesNotContain(WebAssembly.GetName().Name, references);
    }

    [Fact]
    public void Domain_Should_Not_Reference_AspNetCore_Assemblies()
    {
        var references = GetReferencedAssemblyNames(DomainAssembly);

        Assert.DoesNotContain(references, name =>
            name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Infrastructure_Should_Reference_Domain_But_Not_Web()
    {
        var references = GetReferencedAssemblyNames(InfrastructureAssembly);

        Assert.Contains(DomainAssembly.GetName().Name, references);
        Assert.DoesNotContain(WebAssembly.GetName().Name, references);
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_AspNetCore_Mvc_Assemblies()
    {
        var references = GetReferencedAssemblyNames(InfrastructureAssembly);

        Assert.DoesNotContain(references, name =>
            name.StartsWith("Microsoft.AspNetCore.Mvc", StringComparison.Ordinal));
    }

    [Fact]
    public void Web_Should_Reference_Domain_And_Infrastructure()
    {
        var references = GetReferencedAssemblyNames(WebAssembly);

        Assert.Contains(DomainAssembly.GetName().Name, references);
        Assert.Contains(InfrastructureAssembly.GetName().Name, references);
    }

    [Fact]
    public void Controllers_Should_Reside_In_Features()
    {
        var result = Types.InAssembly(WebAssembly)
            .That()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.Controller))
            .Should()
            .ResideInNamespaceStartingWith("WeatherApp.Features")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Handlers_Should_Reside_In_Features()
    {
        var result = Types.InAssembly(WebAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .DoNotHaveNameEndingWith("Tests")
            .Should()
            .ResideInNamespaceStartingWith("WeatherApp.Features")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Feature_Handlers_Should_Not_Depend_On_Mvc_Controller_Base()
    {
        // Handlers are application logic and must not take a dependency on MVC controller types.
        var result = Types.InAssembly(WebAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .ResideInNamespaceStartingWith("WeatherApp.Features")
            .ShouldNot()
            .HaveDependencyOn("Microsoft.AspNetCore.Mvc.Controller")
            .GetResult();

        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Domain_And_Infrastructure_Should_Not_Contain_Feature_Types()
    {
        Assert.Empty(
            Types.InAssembly(DomainAssembly)
                .That()
                .ResideInNamespaceStartingWith("WeatherApp.Features")
                .GetTypes());

        Assert.Empty(
            Types.InAssembly(InfrastructureAssembly)
                .That()
                .ResideInNamespaceStartingWith("WeatherApp.Features")
                .GetTypes());
    }

    [Fact]
    public void Weather_Abstractions_Should_Live_In_Expected_Projects()
    {
        Assert.Equal("WeatherApp.Domain", typeof(Location).Assembly.GetName().Name);
        Assert.Equal("WeatherApp.Domain", typeof(WeatherReading).Assembly.GetName().Name);
        Assert.Equal("WeatherApp.Infrastructure", typeof(IWeatherClient).Assembly.GetName().Name);
        Assert.Equal("WeatherApp.Infrastructure", typeof(IFavoritesStore).Assembly.GetName().Name);
        Assert.Equal("WeatherApp", typeof(Program).Assembly.GetName().Name);
    }

    [Fact]
    public void Feature_View_Location_Expander_Should_Live_In_Web_Project()
    {
        Assert.Equal(
            "WeatherApp",
            typeof(WeatherApp.Razor.FeatureViewLocationExpander).Assembly.GetName().Name);
    }

    private static IReadOnlyCollection<string> GetReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToArray();

    private static string FormatFailures(TestResult result)
    {
        if (result.IsSuccessful || result.FailingTypes is null)
        {
            return "Architecture rule failed.";
        }

        return "Architecture rule failed for: "
            + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
    }
}
