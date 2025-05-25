namespace Jellyfin.Plugin.AniStream.Models;

/// <summary>
/// Represents the scraped information for an anime.
/// </summary>
public class ScrapedAnimeInfo
{
    /// <summary>
    /// Gets or sets the source ID of the anime.
    /// </summary>
    public string? SourceId { get; set; }

    /// <summary>
    /// Gets or sets the title of the anime.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the anime.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the URL of the anime's source page.
    /// </summary>
    public string? SourceUrl { get; set; }
}
