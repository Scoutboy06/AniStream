using System;
using System.Threading.Tasks;
using Jellyfin.Plugin.AniStream.Models;

namespace Jellyfin.Plugin.AniStream.Scrapers;

/// <summary>
/// Base class for anime scrapers.
/// </summary>
public abstract class AnimeScraperBase
{
    /// <summary>
    /// Checks if the scraper can handle the provided URI.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns>A task that represents the asynchronous operation, with a boolean result indicating whether the URI can be handled.</returns>
    public abstract bool CanHandleUri(Uri uri);

    /// <summary>
    /// Gets the anime information by the provided URI.
    /// </summary>
    /// <param name="uri">The URI of the anime to scrape.</param>
    /// <returns>A task that represents the asynchronous operation, with a result of type <see cref="ScrapedAnimeInfo"/> containing the scraped information.</returns>
    public abstract Task<ScrapedAnimeInfo> GetAnimeInfoByUrl(Uri uri);
}
