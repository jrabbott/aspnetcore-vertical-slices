# Contributing

Thanks for improving this sample. Keep changes focused and aligned with Vertical Slice Architecture on ASP.NET Core MVC + Razor.

## Code of conduct

Participation is governed by the [Code of Conduct](CODE_OF_CONDUCT.md).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) matching [`global.json`](global.json) (roll-forward to the latest feature band is allowed)

## Everyday workflow

```bash
dotnet restore aspnetcore-vertical-slices.slnx
dotnet build aspnetcore-vertical-slices.slnx
dotnet test aspnetcore-vertical-slices.slnx
dotnet run --project src/WeatherApp
```

Full detail: [Run, build, and test](docs/how-to/run-build-and-test.md).

## Match CI before you open a PR

CI restores with lock files and treats warnings as errors when `CI=true`. Run the same sequence locally:

```bash
CI=true dotnet restore aspnetcore-vertical-slices.slnx --locked-mode
CI=true dotnet build aspnetcore-vertical-slices.slnx --no-restore
CI=true dotnet test aspnetcore-vertical-slices.slnx --no-build
```

If you change package versions in `Directory.Packages.props`, update the affected `packages.lock.json` files so `--locked-mode` succeeds.

## Where to change code

- New use case: [Add a feature slice](docs/how-to/add-a-feature-slice.md)
- Architecture context: [Vertical Slice Architecture](docs/explanation/vertical-slice-architecture.md)
- Docs map: [docs/README.md](docs/README.md)

## House rules

- **UK spelling** in prose, UI copy, and identifiers (e.g. Favourite, organisation). See [Design choices](docs/explanation/design-choices.md).
- When behaviour or public docs change, update the matching Diátaxis page and the tables in [docs/README.md](docs/README.md) and the root [README.md](README.md) when you add a page.
- Keep [`.agents/skills`](.agents/skills) aligned when you add slices or docs that those skills describe.
- Respect architecture test boundaries (feature isolation, layered dependencies).

## Pull requests

1. Fork (or branch), make a focused change, and open a PR against `main`.
2. Ensure the CI-parity commands above pass.
3. Use the PR checklist (tests, docs, lock files, spelling, architecture).
4. Link related issues when applicable.

## Security

Do not open public issues for vulnerabilities. See [SECURITY.md](SECURITY.md).
