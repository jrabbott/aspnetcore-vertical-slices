using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace WeatherApp.Integration.Tests;

internal static class HtmlDocument
{
    private static readonly IBrowsingContext Context = BrowsingContext.New(Configuration.Default);

    public static async Task<IDocument> ParseAsync(HttpResponseMessage response)
    {
        string html = await response.Content.ReadAsStringAsync();
        return await Context.OpenAsync(request => request.Content(html));
    }

    public static string AntiForgeryToken(IDocument document)
    {
        IHtmlInputElement? input = document.QuerySelector<IHtmlInputElement>("input[name='__RequestVerificationToken']");
        Assert.NotNull(input);
        Assert.False(string.IsNullOrWhiteSpace(input.Value));
        return input.Value;
    }
}
