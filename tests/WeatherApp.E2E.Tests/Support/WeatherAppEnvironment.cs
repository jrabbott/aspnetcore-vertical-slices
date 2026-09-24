using System.Text;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using WireMock.Net.Testcontainers;

namespace WeatherApp.E2E.Tests.Support;

/// <summary>
/// Resolves the site under test: either <c>E2E_BASE_URL</c> or Testcontainers (WireMock + app image).
/// The app reaches WireMock through published host ports (<c>host.docker.internal</c>) so e2e does not
/// depend on container-to-container bridge networking.
/// </summary>
internal sealed class WeatherAppEnvironment : IAsyncDisposable
{
    private readonly WireMockContainer? _wireMock;
    private readonly IContainer? _app;

    private WeatherAppEnvironment(string baseUrl, WireMockContainer? wireMock, IContainer? app)
    {
        BaseUrl = baseUrl;
        _wireMock = wireMock;
        _app = app;
    }

    public string BaseUrl
    {
        get;
    }

    public static async Task<WeatherAppEnvironment> StartAsync(CancellationToken cancellationToken = default)
    {
        if (E2EConfiguration.UseDeployedSite)
        {
            return new WeatherAppEnvironment(E2EConfiguration.DeployedBaseUrl!, wireMock: null, app: null);
        }

        WireMockContainer wireMock = new WireMockContainerBuilder()
            .WithLinuxImage()
            .Build();
        await wireMock.StartAsync(cancellationToken).ConfigureAwait(false);
        await RegisterStubsAsync(wireMock, cancellationToken).ConfigureAwait(false);

        Uri wireMockUri = new(wireMock.GetPublicUrl());
        string geocodingBase = $"http://host.docker.internal:{wireMockUri.Port}/v1/search";
        string forecastBase = $"http://host.docker.internal:{wireMockUri.Port}/v1/forecast";

        IContainer app = new ContainerBuilder(E2EConfiguration.Image)
            .WithPortBinding(8080, assignRandomHostPort: true)
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
            .WithEnvironment("OpenMeteo__GeocodingBaseUrl", geocodingBase)
            .WithEnvironment("OpenMeteo__ForecastBaseUrl", forecastBase)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(8080)
                    .ForPath("/health")))
            .Build();

        await app.StartAsync(cancellationToken).ConfigureAwait(false);

        ushort hostPort = app.GetMappedPublicPort(8080);
        string baseUrl = $"http://127.0.0.1:{hostPort}";
        return new WeatherAppEnvironment(baseUrl, wireMock, app);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync().ConfigureAwait(false);
        }

        if (_wireMock is not null)
        {
            await _wireMock.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async Task RegisterStubsAsync(WireMockContainer wireMock, CancellationToken cancellationToken)
    {
        using HttpClient client = wireMock.CreateClient();
        await PostMappingAsync(client, BuildGeocodeLondonMapping(), cancellationToken).ConfigureAwait(false);
        await PostMappingAsync(client, BuildGeocodeMissMapping(), cancellationToken).ConfigureAwait(false);
        await PostMappingAsync(client, BuildCurrentForecastMapping(), cancellationToken).ConfigureAwait(false);
        await PostMappingAsync(client, BuildDailyForecastMapping(), cancellationToken).ConfigureAwait(false);
    }

    private static async Task PostMappingAsync(HttpClient client, object mapping, CancellationToken cancellationToken)
    {
        string json = JsonSerializer.Serialize(mapping);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await client
            .PostAsync("/__admin/mappings", content, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    private static object BuildGeocodeLondonMapping()
    {
        return new
        {
            Priority = 1,
            Request = new
            {
                Methods = new[]
                {
                    "GET"
                },
                Path = new
                {
                    Matchers = new[]
                    {
                        new
                        {
                            Name = "WildcardMatcher",
                            Pattern = "/v1/search",
                            IgnoreCase = true
                        }
                    }
                },
                Params = new[]
                {
                    new
                    {
                        Name = "name",
                        Matchers = new[]
                        {
                            new
                            {
                                Name = "WildcardMatcher",
                                Pattern = "London",
                                IgnoreCase = true
                            }
                        }
                    }
                }
            },
            Response = new
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                },
                BodyAsJson = new
                {
                    results = new[]
                    {
                        new
                        {
                            name = E2EConfiguration.StubCity,
                            country = E2EConfiguration.StubCountry,
                            latitude = 51.5074,
                            longitude = -0.1278
                        }
                    }
                }
            }
        };
    }

    private static object BuildGeocodeMissMapping()
    {
        return new
        {
            Priority = 10,
            Request = new
            {
                Methods = new[]
                {
                    "GET"
                },
                Path = new
                {
                    Matchers = new[]
                    {
                        new
                        {
                            Name = "WildcardMatcher",
                            Pattern = "/v1/search",
                            IgnoreCase = true
                        }
                    }
                }
            },
            Response = new
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                },
                BodyAsJson = new
                {
                    results = Array.Empty<object>()
                }
            }
        };
    }

    private static object BuildCurrentForecastMapping()
    {
        return new
        {
            Priority = 1,
            Request = new
            {
                Methods = new[]
                {
                    "GET"
                },
                Path = new
                {
                    Matchers = new[]
                    {
                        new
                        {
                            Name = "WildcardMatcher",
                            Pattern = "/v1/forecast",
                            IgnoreCase = true
                        }
                    }
                },
                Params = new[]
                {
                    new
                    {
                        Name = "current",
                        Matchers = new[]
                        {
                            new
                            {
                                Name = "WildcardMatcher",
                                Pattern = "*",
                                IgnoreCase = true
                            }
                        }
                    }
                }
            },
            Response = new
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                },
                BodyAsJson = new
                {
                    current = new
                    {
                        time = "2026-09-24T12:00",
                        temperature_2m = (double)E2EConfiguration.StubTemperatureC,
                        relative_humidity_2m = 55,
                        weather_code = 0,
                        wind_speed_10m = 12.0
                    }
                }
            }
        };
    }

    private static object BuildDailyForecastMapping()
    {
        return new
        {
            Priority = 1,
            Request = new
            {
                Methods = new[]
                {
                    "GET"
                },
                Path = new
                {
                    Matchers = new[]
                    {
                        new
                        {
                            Name = "WildcardMatcher",
                            Pattern = "/v1/forecast",
                            IgnoreCase = true
                        }
                    }
                },
                Params = new[]
                {
                    new
                    {
                        Name = "daily",
                        Matchers = new[]
                        {
                            new
                            {
                                Name = "WildcardMatcher",
                                Pattern = "*",
                                IgnoreCase = true
                            }
                        }
                    }
                }
            },
            Response = new
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                },
                BodyAsJson = new
                {
                    daily = new
                    {
                        time = new[]
                        {
                            "2026-09-24",
                            "2026-09-25",
                            "2026-09-26"
                        },
                        weather_code = new[]
                        {
                            0,
                            2,
                            3
                        },
                        temperature_2m_max = new[]
                        {
                            21.0,
                            18.0,
                            16.0
                        },
                        relative_humidity_2m_mean = new[]
                        {
                            55.0,
                            60.0,
                            62.0
                        },
                        wind_speed_10m_max = new[]
                        {
                            12.0,
                            14.0,
                            10.0
                        }
                    }
                }
            }
        };
    }
}
