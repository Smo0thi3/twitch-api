using FastEndpoints;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TwitchAPI.Endpoints;
using TwitchAPI.Models.Exceptions;
using TwitchAPI.Models.Requests;
using TwitchAPI.Models.Responses;
using TwitchAPI.Services;
using Xunit;

namespace TwitchAPI.Tests.Unit.Endpoints;

public sealed class GetUserEndpointTests
{
    private readonly Mock<ITwitchService> _twitchServiceMock = new();

    private GetUserEndpoint CreateEndpoint()
        => Factory.Create<GetUserEndpoint>(ctx =>
            ctx.AddTestServices(s => s.AddSingleton(_twitchServiceMock.Object)));

    [Fact]
    public async Task GetUser_ValidId_Returns200WithUser()
    {
        // Arrange
        var expectedUser = new UserResponse
        {
            Id = "141981764",
            Login = "twitchdev",
            DisplayName = "TwitchDev",
            Type = "",
            BroadcasterType = "partner",
            Description = "Supporting third-party developers.",
            ProfileImageUrl = "https://example.com/profile.png",
            OfflineImageUrl = "https://example.com/offline.png",
            ViewCount = 5980557,
            CreatedAt = "2016-12-14T20:32:28Z"
        };

        _twitchServiceMock
            .Setup(s => s.GetUserAsync("141981764", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        var ep = CreateEndpoint();
        var req = new GetUserRequest { Id = "141981764" };

        // Act
        await ep.HandleAsync(req, default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(200);
        ep.Response.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUser_UserNotFound_Returns404WithErrorMessage()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetUserAsync("999", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse?)null);

        var ep = CreateEndpoint();
        var req = new GetUserRequest { Id = "999" };

        // Act
        await ep.HandleAsync(req, default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetUser_TwitchUnauthorized_Returns401WithErrorMessage()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TwitchUnauthorizedException());

        var ep = CreateEndpoint();
        var req = new GetUserRequest { Id = "123" };

        // Act
        await ep.HandleAsync(req, default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetUser_ServiceThrowsException_Returns500WithErrorMessage()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        var ep = CreateEndpoint();
        var req = new GetUserRequest { Id = "123" };

        // Act
        await ep.HandleAsync(req, default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(500);
    }
}