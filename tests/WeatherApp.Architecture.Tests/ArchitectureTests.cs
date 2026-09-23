using System.Reflection;
using NetArchTest.Rules;
using WeatherApp.Domain.Weather;
using WeatherApp.Infrastructure.Favourites;
using WeatherApp.Infrastructure.Weather;

namespace WeatherApp.Architecture.Tests;

public sealed class ArchitectureTests
{
    private static readonly Assembly _domainAssembly = typeof(Location).Assembly;
    private static readonly Assembly _infrastructureAssembly = typeof(IWeatherClient).Assembly;
    private static readonly Assembly _webAssembly = typeof(Program).Assembly;

    [Fact]
    public void Domain_Should_Not_Reference_Infrastructure_Or_Web_Assemblies()
    {
        string[] references = GetReferencedAssemblyNames(_domainAssembly);

        Assert.DoesNotContain(_infrastructureAssembly.GetName().Name, references);
        Assert.DoesNotContain(_webAssembly.GetName().Name, references);
    }

    [Fact]
    public void Domain_Should_Not_Reference_AspNetCore_Assemblies()
    {
        string[] references = GetReferencedAssemblyNames(_domainAssembly);

        Assert.DoesNotContain(references, name =>
            name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Infrastructure_Should_Reference_Domain_But_Not_Web()
    {
        string[] references = GetReferencedAssemblyNames(_infrastructureAssembly);

        Assert.Contains(_domainAssembly.GetName().Name, references);
        Assert.DoesNotContain(_webAssembly.GetName().Name, references);
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_AspNetCore_Mvc_Assemblies()
    {
        string[] references = GetReferencedAssemblyNames(_infrastructureAssembly);

        Assert.DoesNotContain(references, name =>
            name.StartsWith("Microsoft.AspNetCore.Mvc", StringComparison.Ordinal));
    }

    [Fact]
    public void Web_Should_Reference_Domain_And_Infrastructure()
    {
        string[] references = GetReferencedAssemblyNames(_webAssembly);

        Assert.Contains(_domainAssembly.GetName().Name, references);
        Assert.Contains(_infrastructureAssembly.GetName().Name, references);
    }

    [Fact]
    public void Controllers_Should_Reside_In_Features()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(_webAssembly)
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
        NetArchTest.Rules.TestResult result = Types.InAssembly(_webAssembly)
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
        NetArchTest.Rules.TestResult result = Types.InAssembly(_webAssembly)
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
            Types.InAssembly(_domainAssembly)
                .That()
                .ResideInNamespaceStartingWith("WeatherApp.Features")
                .GetTypes());

        Assert.Empty(
            Types.InAssembly(_infrastructureAssembly)
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
        Assert.Equal("WeatherApp.Infrastructure", typeof(IFavouritesStore).Assembly.GetName().Name);
        Assert.Equal("WeatherApp", typeof(Program).Assembly.GetName().Name);
    }

    [Fact]
    public void Feature_View_Location_Expander_Should_Live_In_Web_Project()
    {
        Assert.Equal(
            "WeatherApp",
            typeof(WeatherApp.Razor.FeatureViewLocationExpander).Assembly.GetName().Name);
    }

    private static string[] GetReferencedAssemblyNames(Assembly assembly)
    {
        return [.. assembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)];
    }

    private static string FormatFailures(NetArchTest.Rules.TestResult result)
    {
        return result.IsSuccessful || result.FailingTypes is null
            ? "Architecture rule failed."
            : "Architecture rule failed for: "
            + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
    }
}
