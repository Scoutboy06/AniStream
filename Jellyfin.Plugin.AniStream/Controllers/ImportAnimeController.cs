using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Jellyfin.Plugin.AniStream.Models;
using Jellyfin.Plugin.AniStream.Scrapers;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.AniStream.Controllers;

/// <summary>
/// Controller for importing anime data into AniStream.
/// </summary>
[ApiController]
[Route("AniStream")]
[Produces(MediaTypeNames.Application.Json)]
public class ImportAnimeController : ControllerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportAnimeController"/> class.
    /// </summary>
    public ImportAnimeController()
    {
    }

    /// <summary>
    /// Imports anime data.
    /// </summary>
    /// <returns>A success message.</returns>
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok("👋 from AniStream");
    }

    /// <summary>
    /// Imports anime data.
    /// </summary>
    /// <param name="request">The request containing import details.</param>
    /// <returns>A success message.</returns>
    [HttpPost("import")]
    public async Task<IActionResult> ImportAnime([FromBody] ImportAnimeRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        Uri uri = new Uri(request.SourceUrl);
        if (!new GogoAnime().CanHandleUri(uri))
        {
            return BadRequest("No suitable scraper found for the provided URL.");
        }

        ScrapedAnimeInfo? animeInfo = await new GogoAnime().GetAnimeInfoByUrl(uri).ConfigureAwait(false);
        if (animeInfo == null)
        {
            return BadRequest("Failed to scrape anime information from the provided URL.");
        }

        return Ok(new
        {
            Message = "Anime information scraped successfully.",
            data = new
            {
                Title = animeInfo.Title,
            },
            request = new
            {
                SourceUrl = request.SourceUrl,
                LibraryId = request.LibraryId,
                LibraryType = request.LibraryType
            }
        });
    }
}
