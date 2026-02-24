using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchAPI.Tests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public TwitchApiMockHandler MockHandler { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace the real HttpClient used by TwitchService with one that uses our mock handler
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IHttpClientFactory));

            // Remove existing HttpClient registrations for TwitchService
            var httpClientDescriptors = services
                .Where(d => d.ServiceType.Name.Contains("HttpClient") ||
                            d.ImplementationType?.Name.Contains("TwitchService") == true)
                .ToList();

            foreach (var d in httpClientDescriptors)
                services.Remove(d);

            // Re-register TwitchService HttpClient with the mock handler
            services
                .AddHttpClient<Services.ITwitchService, Services.TwitchService>()
                .AddHttpMessageHandler(() => MockHandler);
        });

        builder.UseSetting("Twitch:ClientId", "test-client-id");
        builder.UseSetting("Twitch:ClientSecret", "test-client-secret");
        builder.UseSetting("Twitch:AuthUrl", "https://id.twitch.tv/oauth2/token");
        builder.UseSetting("Twitch:ApiBaseUrl", "https://api.twitch.tv/helix");
    }
}
