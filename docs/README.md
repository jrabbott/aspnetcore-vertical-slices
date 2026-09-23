# Documentation

Docs follow the [Diátaxis](https://diataxis.fr/) system: four types of content for four needs. Pick the quadrant that matches what you are trying to do.

**For contributors:** [CONTRIBUTING.md](../CONTRIBUTING.md) covers prerequisites, CI-parity commands, and pull requests. Prose and identifiers use **UK spelling** (e.g. Favourite, organisation).

| Need | Type | Question it answers |
|---|---|---|
| Learn by doing | **Tutorials** | “Teach me to use this sample.” |
| Get a job done | **How-to guides** | “How do I achieve X?” |
| Look something up | **Reference** | “What are the exact facts?” |
| Understand | **Explanation** | “Why is it like this?” |

## Tutorials

Oriented to learning. Follow a path; finish with a working mental model of the running app.

| Doc | Description |
|---|---|
| [Run the weather app](tutorials/run-the-weather-app.md) | Restore, run, search, favourites, and tests — first success |

## How-to guides

Oriented to goals. Steps assume you already know what you want.

| Doc | Description |
|---|---|
| [Add a feature slice](how-to/add-a-feature-slice.md) | Extend the app with a new VSA use case |
| [Run, build, and test](how-to/run-build-and-test.md) | Everyday restore / build / run / test (including CI-like) |
| [Replace the weather provider](how-to/replace-the-weather-provider.md) | Swap Open-Meteo behind `IWeatherClient` |

## Reference

Oriented to information. Accurate, concise descriptions you can scan.

| Doc | Description |
|---|---|
| [Routes and endpoints](reference/routes.md) | HTTP methods, paths, controllers |
| [Repository layout](reference/repository-layout.md) | Folders and what belongs where |
| [Build system and packages](reference/build-and-packages.md) | CPM, lock files, analyzers, CI, Dependabot, MTP |
| [Test projects](reference/testing.md) | Test assemblies and architecture rules |

## Explanation

Oriented to understanding. Context, trade-offs, and design rationale.

| Doc | Description |
|---|---|
| [Vertical Slice Architecture](explanation/vertical-slice-architecture.md) | VSA on MVC, handlers, boundaries, DI |
| [Razor view discovery](explanation/razor-view-discovery.md) | Feature views + why `_ViewImports` / `_ViewStart` stay at the web root |
| [Design choices](explanation/design-choices.md) | Goals, no MediatR/AutoMapper, Open-Meteo, session favourites |

The root [README](../README.md) remains the short repository entry point.
