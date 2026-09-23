# How to run with Docker

Goal: package a **already published** WeatherApp into a runtime container image (build once), then run it locally or pull the image published by CD to GitHub Container Registry.

## Build once

Compile and test on the host (or in CI). The [`Dockerfile`](../../Dockerfile) is **runtime-only** — it must not run `dotnet restore`, `build`, or `publish`. That keeps the image identical to the bits you just tested.

## Local: publish, then build the image

```bash
dotnet restore aspnetcore-vertical-slices.slnx --locked-mode
dotnet build aspnetcore-vertical-slices.slnx --configuration Release --no-restore
dotnet test aspnetcore-vertical-slices.slnx --configuration Release --no-build
dotnet publish src/WeatherApp/WeatherApp.csproj --configuration Release --no-build -o ./.publish/web

cp .dockerignore .publish/web/.dockerignore
docker build -f Dockerfile -t weatherapp:local ./.publish/web
```

`./.publish/` is gitignored. Publish output is the Docker **build context** (not the repository root).

## Run the container

```bash
docker run --rm -p 8080:8080 weatherapp:local
```

- App: <http://127.0.0.1:8080/weather/search>
- Health (for probes such as Azure Container Apps): <http://127.0.0.1:8080/health>

The process listens on HTTP port **8080**. TLS belongs at the ingress (for example Azure Container Apps). The app trusts `X-Forwarded-For` / `X-Forwarded-Proto` so HTTPS redirection and HSTS see the external scheme.

## CI and CD

| Workflow | When | What |
|---|---|---|
| [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml) | Pull requests | Shared [build-test-publish](../../.github/actions/build-test-publish/action.yml) action, then `docker build` (no push) and a `/health` smoke check |
| [`.github/workflows/cd.yml`](../../.github/workflows/cd.yml) | Push to `main` (and `workflow_dispatch`) | Same action, then push the image to GHCR |

Image name:

```text
ghcr.io/<owner>/<repository>/weatherapp
```

Tags on `main`: `latest` and `sha-<short>`.

Pull example (public package or after `docker login ghcr.io`):

```bash
docker pull ghcr.io/<owner>/<repository>/weatherapp:latest
docker run --rm -p 8080:8080 ghcr.io/<owner>/<repository>/weatherapp:latest
```

Package visibility is controlled under the repository’s GitHub Packages settings.

## Operations notes for Azure Container Apps

- Allow outbound HTTPS to Open-Meteo (`geocoding-api.open-meteo.com`, `api.open-meteo.com`).
- Favourites use an in-memory session store. Prefer a **single replica**, or enable session affinity, unless you introduce a shared store.
- Point liveness/readiness probes at `GET /health`.

## Related

- [Run, build, and test](run-build-and-test.md)
- [Build system and packages](../reference/build-and-packages.md)
- [Routes and endpoints](../reference/routes.md)
