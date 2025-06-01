using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jellyfin.Plugin.AniStream.Models;

namespace Jellyfin.Plugin.AniStream.Scrapers;

/// <summary>
/// Scraper for GogoAnime.
/// </summary>
public class GogoAnime : AnimeScraperBase
{
    private static readonly GogoAnime _instance = new GogoAnime();

    /// <summary>
    /// Gets the singleton instance of the GogoAnime scraper.
    /// </summary>
    /// <returns>The singleton instance of the GogoAnime scraper.</returns>
    public static GogoAnime Instance => _instance;

    /// <inheritdoc />
    public override bool CanHandleUri(Uri uri)
    {
        // Check if the URL is a GogoAnime URL
        return uri.Host.Equals("gogoanime", StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override async Task<ScrapedAnimeInfo> GetAnimeInfoByUrl(Uri uri)
    {
        if (uri == null || !CanHandleUri(uri))
        {
            throw new NotSupportedException("This uri is not supported by the GogoAnime scraper.");
        }

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(html);

        string? title = document.DocumentNode.SelectSingleNode("//h2")?.InnerText;
        if (title != null)
        {
            int index = title.LastIndexOf("Episode", StringComparison.Ordinal);
            if (index != -1)
            {
                title = title.Substring(0, index).Trim();
            }
        }

        // div.info-content div.spe span a
        string? description = document.DocumentNode.SelectSingleNode("//div[@class='info-content']//div[@class='spe']//span//a")?.InnerText.Trim();

        return new ScrapedAnimeInfo
        {
            SourceUrl = uri.ToString(),
            SourceId = uri.AbsolutePath.TrimStart('/'),
            Description = description,
            Title = title,
        };
    }
}
