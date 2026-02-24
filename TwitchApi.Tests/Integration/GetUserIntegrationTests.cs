using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TwitchAPI.Constants;
using TwitchAPI.Models.Responses;
using Xunit;

namespace TwitchAPI.Tests.Integration;

public sealed class GetUserIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TwitchApiMockHandler _mockHandler;

    // Token response reused across all tests
    private static readonly object TokenResponse = new
    {
        access_token = "fake-token",
        expires_in = 9999999,
        token_type = "bearer"
    };

    public GetUserIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _mockHandler = factory.MockHandler;

        // Auth call always succeeds
        _mockHandler.SetupResponse("oauth2/token", HttpStatusCode.OK, TokenResponse);
    }

    [Fact]
    public async Task GetUser_ValidId_Returns200WithUser()
    {
        // Arrange
        var twitchResponse = new
        {
            data = new[]
            {
                new
                {
                    id = "141981764",
                    login = "twitchdev",
                    display_name = "TwitchDev",
                    type = "",
                    broadcaster_type = "partner",
                    description = "Supporting third-party developers.",
                    profile_image_url = "https://example.com/profile.png",
                    offline_image_url = "https://example.com/offline.png",
                    view_count = 5980557,
                    created_at = "2016-12-14T20:32:28Z"
                }
            }
        };
        _mockHandler.SetupResponse("helix/users", HttpStatusCode.OK, twitchResponse);

        // Act
        var response = await _client.GetAsync("/analytics/user?id=141981764");
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Id.Should().Be("141981764");
        result.Login.Should().Be("twitchdev");
        result.DisplayName.Should().Be("TwitchDev");
    }

    [Fact]
    public async Task GetUser_MissingId_Returns400WithErrorMessage()
    {
        // Act
        var response = await _client.GetAsync("/analytics/user");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result!.Error.Should().Be(ErrorMessages.InvalidOrMissingId);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.34")]
    [InlineData("123abc")]
    public async Task GetUser_InvalidIdFormat_Returns400WithErrorMessage(string invalidId)
    {
        // Act
        var response = await _client.GetAsync($"/analytics/user?id={invalidId}");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result!.Error.Should().Be(ErrorMessages.InvalidOrMissingId);
    }

    [Fact]
    public async Task GetUser_UserNotFound_Returns404WithErrorMessage()
    {
        // Arrange
        _mockHandler.SetupResponse("helix/users", HttpStatusCode.OK, new { data = Array.Empty<object>() });

        // Act
        var response = await _client.GetAsync("/analytics/user?id=999");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result!.Error.Should().Be(ErrorMessages.UserNotFound);
    }

    [Fact]
    public async Task GetUser_TwitchReturnsUnauthorized_Returns401WithErrorMessage()
    {
        // Arrange
        _mockHandler.SetupResponse("helix/users", HttpStatusCode.Unauthorized);

        // Act
        var response = await _client.GetAsync("/analytics/user?id=123");
        var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result!.Error.Should().Be(ErrorMessages.Unauthorized);
    }
}
