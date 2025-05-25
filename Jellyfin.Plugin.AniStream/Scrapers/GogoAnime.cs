using System;
using System.Threading.Tasks;
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
    public override Task<ScrapedAnimeInfo> GetAnimeInfoByUrl(string url)
    {
        // Implement the logic to scrape anime information from GogoAnime
        throw new NotImplementedException("GogoAnime scraping logic is not implemented yet.");
    }
}
