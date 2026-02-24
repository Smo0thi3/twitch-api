using FastEndpoints;
using TwitchAPI.Constants;
using TwitchAPI.Models.Requests;
using TwitchAPI.Services;
using TwitchAPI.Models.Exceptions;
using ErrorResponse = TwitchAPI.Models.Responses.ErrorResponse;

namespace TwitchAPI.Endpoints;

public sealed class GetUserEndpoint : Endpoint<GetUserRequest>
{
    private readonly ITwitchService _twitchService;

    public GetUserEndpoint(ITwitchService twitchService)
    {
        _twitchService = twitchService;
    }

    public override void Configure()
    {
        Get("/analytics/user");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _twitchService.GetUserAsync(req.Id, ct);

            if (user is null)
            {
                await Send.ResponseAsync(ErrorResponse.From(ErrorMessages.UserNotFound), StatusCodes.Status404NotFound, cancellation: ct);
                return;
            }

            await Send.OkAsync(user, ct);
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