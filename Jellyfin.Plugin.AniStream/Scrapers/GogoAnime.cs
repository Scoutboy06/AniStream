using System;
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
    public override bool CanHandleUrl(string url)
    {
        // Check if the URL is a GogoAnime URL
        return url.StartsWith("https://gogoanime", StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override async Task<ScrapedAnimeInfo> GetAnimeInfoByUrl(string url)
    {
        if (!CanHandleUrl(url))
        {
            throw new NotSupportedException("This url is not supported by the GogoAnime scraper.");
        }

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
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

            return new ScrapedAnimeInfo
            {
                Title = title,
            };
        }
    }
}
