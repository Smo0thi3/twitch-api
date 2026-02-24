using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TwitchAPI.Constants;
using TwitchAPI.Models.Responses;
using Xunit;

namespace TwitchAPI.Tests.Integration;

public sealed class GetStreamsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TwitchApiMockHandler _mockHandler;

    private static readonly object TokenResponse = new
    {
        access_token = "fake-token",
        expires_in = 9999999,
        token_type = "bearer"
    };

    public GetStreamsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _mockHandler = factory.MockHandler;

        _mockHandler.SetupResponse("oauth2/token", HttpStatusCode.OK, TokenResponse);
    }

    [Fact]
    public async Task GetStreams_Returns200WithStreamList()
    {
        // Arrange
        var twitchResponse = new
        {
            data = new[]
            {
                new { title = "Stream One", user_name = "UserOne", id = "1", user_id = "u1", user_login = "userone",
                      game_id = "1", game_name = "Game", type = "live", viewer_count = 100,
                      started_at = "2024-01-01T00:00:00Z", language = "en", thumbnail_url = "", is_mature = false },
                new { title = "Stream Two", user_name = "UserTwo", id = "2", user_id = "u2", user_login = "usertwo",
                      game_id = "2", game_name = "Game2", type = "live", viewer_count = 200,
                      started_at = "2024-01-01T00:00:00Z", language = "es", thumbnail_url = "", is_mature = false }
            },
            pagination = new { cursor = "" }
        };
        _mockHandler.SetupResponse("helix/streams", HttpStatusCode.OK, twitchResponse);

        // Act
        var response = await _client.GetAsync("/analytics/streams");
        var result = await response.Content.ReadFromJsonAsync<List<StreamResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCount(2);
        result![0].Title.Should().Be("Stream One");
        result![0].UserName.Should().Be("UserOne");
        result![1].Title.Should().Be("Stream Two");
        result![1].UserName.Should().Be("UserTwo");
    }

    [Fact]
    public async Task GetStreams_Returns200WithEmptyList()
    {
        // Arrange
        _mockHandler.SetupResponse("helix/streams", HttpStatusCode.OK, new { data = Array.Empty<object>(), pagination = new { } });

        // Act
        var response = await _client.GetAsync("/analytics/streams");
        var result = await response.Content.ReadFromJsonAsync<List<StreamResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStreams_TwitchReturnsUnauthorized_Returns401WithErrorMessage()
    {
        // Arrange
        _mockHandler.SetupResponse("helix/streams", HttpStatusCode.Unauthorized);

        // Act
        var response = await _client.GetAsync("/analytics/streams");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result!.Error.Should().Be(ErrorMessages.Unauthorized);
    }

    [Fact]
    public async Task GetStreams_TwitchReturnsServerError_Returns500WithErrorMessage()
    {
        // Arrange
        _mockHandler.SetupResponse("helix/streams", HttpStatusCode.InternalServerError);

        // Act
        var response = await _client.GetAsync("/analytics/streams");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result!.Error.Should().Be(ErrorMessages.InternalServerError);
    }
}
