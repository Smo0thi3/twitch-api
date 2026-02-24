using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TwitchAPI.Configuration;
using TwitchAPI.Models.Responses;
using TwitchAPI.Models.Exceptions;

namespace TwitchAPI.Services;

public interface ITwitchService
{
    Task<UserResponse?> GetUserAsync(string id, CancellationToken cancellationToken = default);
    Task<List<StreamResponse>> GetStreamsAsync(CancellationToken cancellationToken = default);
}

public sealed class TwitchService : ITwitchService
{
    private readonly HttpClient _httpClient;
    private readonly TwitchSettings _settings;
    private readonly ILogger<TwitchService> _logger;

    public TwitchService(HttpClient httpClient, IOptions<TwitchSettings> settings, ILogger<TwitchService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<UserResponse?> GetUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var token = await GetAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.ApiBaseUrl}/users?id={id}");
        SetTwitchHeaders(request, token);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new TwitchUnauthorizedException();

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var twitchResponse = JsonSerializer.Deserialize<TwitchUsersResponse>(content);

        return twitchResponse?.Data.FirstOrDefault();
    }

    public async Task<List<StreamResponse>> GetStreamsAsync(CancellationToken cancellationToken = default)
    {
        var token = await GetAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.ApiBaseUrl}/streams?type=live");
        SetTwitchHeaders(request, token);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new TwitchUnauthorizedException();

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var twitchResponse = JsonSerializer.Deserialize<TwitchStreamsResponse>(content);

        return twitchResponse?.Data ?? [];
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var formContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", _settings.ClientId),
            new KeyValuePair<string, string>("client_secret", _settings.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        ]);

        using var authRequest = new HttpRequestMessage(HttpMethod.Post, _settings.AuthUrl)
        {
            Content = formContent
        };

        var authResponse = await _httpClient.SendAsync(authRequest, cancellationToken);
        authResponse.EnsureSuccessStatusCode();

        var authContent = await authResponse.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TwitchTokenResponse>(authContent);

        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to retrieve Twitch access token.");
    }

    private void SetTwitchHeaders(HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Client-Id", _settings.ClientId);
    }
}