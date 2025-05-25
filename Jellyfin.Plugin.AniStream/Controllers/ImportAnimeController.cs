using System.Net.Http.Headers;
using System.Net.Mime;
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
    public IActionResult ImportAnime([FromBody] ImportAnimeRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        if (!new GogoAnime().CanHandleUrl(request.SourceUrl))
        {
            // Handle GogoAnime import logic here
            return Ok(new
            {
                Message = "Source URL is not supported yet.",
                request.SourceUrl,
                request.LibraryId,
                request.LibraryType
            });
        }

        // Perform the import logic here using the request data

        return Ok(new
        {
            Message = "Anime add request received successfully.",
            request.SourceUrl,
            request.LibraryId,
            request.LibraryType
        });
    }
}
