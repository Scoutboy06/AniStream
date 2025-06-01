using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AniStream.Configuration;
using Jellyfin.Plugin.AniStream.Controllers;
using Jellyfin.Plugin.AniStream.Models;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AniStream;

/// <summary>
/// Represents the AniStream plugin for Jellyfin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILogger<Plugin> _logger;
    private readonly ILibraryManager _libraryManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Plugin}"/> interface.</param>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        ILogger<Plugin> logger,
        ILibraryManager libraryManager)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = logger;
        _libraryManager = libraryManager;

        foreach (var r in GetType().Assembly.GetManifestResourceNames())
        {
            _logger.LogInformation("AniStream Resource: {Resource}", r);
        }
    }

    /// <inheritdoc />
    public override string Name => "AniStream";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("9753bc98-85d7-41b6-a928-7922d02a5ced");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        _logger.LogInformation("Registering AniStream plugin pages.");

        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html",
            },
            new PluginPageInfo
            {
                Name = "AniStreamJS",
                EmbeddedResourcePath = GetType().Namespace + ".wwwroot.identify-shows.js",
                MenuSection = "none",
                DisplayName = "AniStream Scripts"
            }
        ];
    }

    /// <summary>
    /// Adds an anime to the library based on the provided scraped anime information.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public void OnApplicationStarting(IApplicationBuilder app)
    {
        var partManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
        partManager.ApplicationParts.Add(
            new AssemblyPart(typeof(ImportAnimeController).Assembly));

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        _logger.LogInformation("AniStream API endpoints registered successfully.");
    }

    /// <summary>
    /// Called when the application is stopped.
    /// This method is used to perform any necessary cleanup or finalization tasks.
    /// </summary>
    public void OnApplicationStopped()
    {
        _logger.LogInformation("AniStream plugin has stopped.");
    }

    /// <summary>
    /// Adds an anime to the library based on the provided scraped anime information.
    /// </summary>
    /// <param name="animeInfo">The scraped anime information.</param>
    /// <param name="libraryId">The ID of the library to which the anime should be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAnimeToLibrary(ScrapedAnimeInfo animeInfo, Guid libraryId)
    {
        try
        {
            _logger.LogInformation("Adding anime '{Title}' to library {LibraryId}", animeInfo.Title, libraryId);

            var library = _libraryManager.GetItemById(libraryId);
            if (library == null)
            {
                _logger.LogError("Library with ID {LibraryId} not found", libraryId);
                return;
            }

            // Create the series directory structure
            var libraryPath = library.Path;
            var seriesPath = Path.Combine(libraryPath, SanitizeFileName(animeInfo.Title ?? string.Empty));

            if (!Directory.Exists(seriesPath))
            {
                Directory.CreateDirectory(seriesPath);
                _logger.LogInformation("Created directory for anime: {SeriesPath}", seriesPath);
            }

            // Create a Series entity for the anime
            var series = new Series
            {
                Name = animeInfo.Title,
                Path = seriesPath,
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            // Set additional metadata if available
            if (!string.IsNullOrEmpty(animeInfo.Description))
            {
                series.Overview = animeInfo.Description;
            }

            // if (animeInfo.Year.HasValue)
            // {
            //     series.ProductionYear = animeInfo.Year.Value;
            // }

            // Add the series to the library
            _libraryManager.CreateItem(series, library);

            // Trigger a library scan to pick up the new item
            await _libraryManager.ValidateMediaLibrary(new Progress<double>(), CancellationToken.None).ConfigureAwait(false);

            _logger.LogInformation("Successfully added anime '{Title}' to library", animeInfo.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add anime '{Title}' to library", animeInfo.Title);
        }
    }

    /// <summary>
    /// Sanitizes a filename by removing invalid characters.
    /// </summary>
    /// <param name="fileName">The filename to sanitize.</param>
    /// <returns>A sanitized filename.</returns>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }
}
