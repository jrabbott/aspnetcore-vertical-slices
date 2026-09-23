using Microsoft.AspNetCore.Http;
using WeatherApp.Mvc;

namespace WeatherApp.Unit.Tests;

public sealed class RequestAcceptsTests
{
    [Theory]
    [InlineData("application/json", true)]
    [InlineData("application/json, text/plain; q=0.9", true)]
    [InlineData("text/html", false)]
    [InlineData("", false)]
    public void Json_DetectsAcceptHeader(string accept, bool expected)
    {
        DefaultHttpContext context = new();
        if (!string.IsNullOrEmpty(accept))
        {
            context.Request.Headers.Accept = accept;
        }

        Assert.Equal(expected, RequestAccepts.Json(context.Request));
    }
}
