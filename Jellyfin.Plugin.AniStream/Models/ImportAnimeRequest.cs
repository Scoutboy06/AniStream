namespace Jellyfin.Plugin.AniStream.Models;

/// <summary>
/// Represents a request to import anime data into AniStream.
/// </summary>
public class ImportAnimeRequest
{
    /// <summary>
    /// Gets or sets the source URL for the anime data import.
    /// </summary>
    public string SourceUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the library ID where the anime data will be imported.
    /// </summary>
    public string LibraryId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of library for the anime data import.
    /// </summary>
    public string LibraryType { get; set; } = string.Empty;
}
