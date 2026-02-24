using FastEndpoints;
using FluentAssertions;
using Moq;
using TwitchAPI.Endpoints;
using TwitchAPI.Models.Exceptions;
using TwitchAPI.Models.Responses;
using TwitchAPI.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchAPI.Tests.Unit.Endpoints;

public sealed class GetStreamsEndpointTests
{
    private readonly Mock<ITwitchService> _twitchServiceMock = new();

    private GetStreamsEndpoint CreateEndpoint()
        => Factory.Create<GetStreamsEndpoint>(ctx =>
            ctx.AddTestServices(s => s.AddSingleton(_twitchServiceMock.Object)));

    [Fact]
    public async Task GetStreams_Returns200WithStreamList()
    {
        // Arrange
        var expectedStreams = new List<StreamResponse>
        {
            new() { Title = "Stream One", UserName = "UserOne" },
            new() { Title = "Stream Two", UserName = "UserTwo" }
        };

        _twitchServiceMock
            .Setup(s => s.GetStreamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedStreams);

        var ep = CreateEndpoint();

        // Act
        await ep.HandleAsync(default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(200);
        ep.Response.Should().BeEquivalentTo(expectedStreams);
    }

    [Fact]
    public async Task GetStreams_Returns200WithEmptyList()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetStreamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var ep = CreateEndpoint();

        // Act
        await ep.HandleAsync(default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(200);
        ep.Response.Should().BeEquivalentTo(new List<StreamResponse>());
    }

    [Fact]
    public async Task GetStreams_TwitchUnauthorized_Returns401()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetStreamsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TwitchUnauthorizedException());

        var ep = CreateEndpoint();

        // Act
        await ep.HandleAsync(default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GetStreams_ServiceThrowsException_Returns500()
    {
        // Arrange
        _twitchServiceMock
            .Setup(s => s.GetStreamsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        var ep = CreateEndpoint();

        // Act
        await ep.HandleAsync(default);

        // Assert
        ep.HttpContext.Response.StatusCode.Should().Be(500);
    }
}