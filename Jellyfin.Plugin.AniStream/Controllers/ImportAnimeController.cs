using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AniStream.Controllers;

/// <summary>
/// Controller for importing anime data into AniStream.
/// </summary>
[ApiController]
[Route("AniStream/[controller]")]
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
    /// <returns>A success message.</returns>
    [HttpPost("import")]
    public IActionResult ImportAnime()
    {
        return Ok("Anime data imported successfully.");
    }
}
