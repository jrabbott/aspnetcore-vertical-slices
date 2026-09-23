# Build system and packages

## Central Package Management

NuGet versions are managed in `Directory.Packages.props`. Project files reference packages **without** `Version` attributes.

Restores use per-project `packages.lock.json` (`RestorePackagesWithLockFile`). CI restores with:

```bash
dotnet restore aspnetcore-vertical-slices.slnx --locked-mode
```

## Directory.Build.props

Shared MSBuild settings enable:

- SDK analyzers (`latest-recommended`)
- code-style enforcement in build
- deterministic builds
- NuGet audit (high severity and above)
- artifacts output under `.artifacts/`
- Microsoft Testing Platform for `*.Tests` projects (`UseMicrosoftTestingPlatformRunner`, `OutputType=Exe`)

When `CI=true` (GitHub Actions):

- `ContinuousIntegrationBuild=true`
- `TreatWarningsAsErrors=true`

## Style and metrics

| File | Role |
|---|---|
| `.editorconfig` | Formatting, naming, nullable gates, analyzer severities |
| `CodeMetricsConfig.txt` | CA1501 / CA1502 / CA1505 / CA1506 thresholds (`AdditionalFiles`) |

## CI and Dependabot

| Path | Role |
|---|---|
| `.github/workflows/ci.yml` | Locked restore, build, test on `main` and pull requests |
| `.github/dependabot.yml` | Grouped NuGet (weekly) and GitHub Actions (monthly) updates |

## SDK and test runner

`global.json` pins the .NET 10 SDK band and sets:

```json
"test": { "runner": "Microsoft.Testing.Platform" }
```

## Related

- [How to run, build, and test](../how-to/run-build-and-test.md)
- [Test projects](testing.md)
