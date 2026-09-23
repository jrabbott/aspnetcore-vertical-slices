# Feature slice checklist

Use while implementing. Replace `<Area>` / `<Slice>` (default Area = `Weather`).

## Files

```text
src/WeatherApp/Features/<Area>/<Slice>/
├── <Slice>Controller.cs
├── <Slice>Request.cs
├── <Slice>Handler.cs
├── <Slice>Response.cs
├── <Slice>RequestValidator.cs   # when input needs validation
└── Index.cshtml                 # page slices only
```

Namespace: `WeatherApp.Features.<Area>.<Slice>`

## DI

In `src/WeatherApp/Hosting/WeatherAppFeatureServiceCollectionExtensions.cs`:

```csharp
services.AddTransient<IValidator<<Slice>Request>, <Slice>RequestValidator>();
services.AddTransient<<Slice>Handler>();
```

## Tests to touch

| Kind | Project | Action |
|---|---|---|
| Unit | `tests/WeatherApp.Unit.Tests` | Handler (± validator) tests |
| Integration | `tests/WeatherApp.Integration.Tests` | Route / DOM tests if HTTP-facing |
| Architecture | `tests/WeatherApp.Architecture.Tests/FeatureBoundaryTests.cs` | Add namespace to slice lists / InlineData |

## Verify

```bash
dotnet test aspnetcore-vertical-slices.slnx
```
