using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.AniStream.Configuration;
using Jellyfin.Plugin.AniStream.Controllers;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Plugin}"/> interface.</param>
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = logger;

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
            }
        ];
    }

    /// <summary>
    /// Called when the application is starting.
    /// This method is used to register the plugin's API endpoints and configure routing.
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
}
