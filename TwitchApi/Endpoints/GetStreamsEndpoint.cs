using FastEndpoints;
using TwitchAPI.Constants;
using TwitchAPI.Services;
using TwitchAPI.Models.Exceptions;
using ErrorResponse = TwitchAPI.Models.Responses.ErrorResponse;

namespace TwitchAPI.Endpoints;

public sealed class GetStreamsEndpoint : EndpointWithoutRequest
{
    private readonly ITwitchService _twitchService;

    public GetStreamsEndpoint(ITwitchService twitchService)
    {
        _twitchService = twitchService;
    }

    public override void Configure()
    {
        Get("/analytics/streams");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var streams = await _twitchService.GetStreamsAsync(ct);
            await Send.OkAsync(streams, ct);
        }
        catch (TwitchUnauthorizedException)
        {
            await Send.ResponseAsync(ErrorResponse.From(ErrorMessages.Unauthorized), StatusCodes.Status401Unauthorized, cancellation: ct);
        }
        catch (Exception)
        {
            await Send.ResponseAsync(ErrorResponse.From(ErrorMessages.InternalServerError), StatusCodes.Status500InternalServerError, cancellation: ct);
        }
    }
}