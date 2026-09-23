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

## Front-end compile (MSBuild)

The web project compiles SCSS and TypeScript during `dotnet build` (no Node):

| Package | Role |
|---|---|
| `AspNetCore.SassCompiler` | `Styles/*.scss` → `wwwroot/css/*.css` (gitignored) |
| `Microsoft.TypeScript.MSBuild` | `Scripts/*.ts` → `wwwroot/js/*.js` (gitignored) |

Generated CSS/JS are not committed; `dotnet build` is required before run/publish so MapStaticAssets can fingerprint the outputs. See [Theme and front-end assets](theme-and-assets.md).

## CI, CD, and Dependabot

| Path | Role |
|---|---|
| `.github/actions/build-test-publish/` | Shared composite: locked restore, Release build, test, publish to `.publish/web` |
| `.github/workflows/ci.yml` | Pull requests: composite action, runtime `docker build`, `/health` smoke (no registry push) |
| `.github/workflows/cd.yml` | Push to `main`: same composite, push image to `ghcr.io/<owner>/<repo>/weatherapp` |
| `Dockerfile` | Runtime-only image; context is publish output (build once) |
| `.github/dependabot.yml` | Grouped NuGet (weekly) and GitHub Actions (monthly) updates |

See [Run with Docker](../how-to/run-with-docker.md).

## SDK and test runner

`global.json` pins the .NET 10 SDK band and sets:

```json
"test": { "runner": "Microsoft.Testing.Platform" }
```

## Related

- [How to run, build, and test](../how-to/run-build-and-test.md)
- [Test projects](testing.md)
