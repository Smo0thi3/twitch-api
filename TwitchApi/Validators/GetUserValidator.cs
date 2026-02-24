using FastEndpoints;
using FluentValidation;
using TwitchAPI.Models.Requests;

namespace TwitchAPI.Validators;

public sealed class GetUserValidator : Validator<GetUserRequest>
{
    public GetUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Invalid or missing 'id' parameter.")
            .Matches(@"^\d+$")
            .WithMessage("Invalid or missing 'id' parameter.");
    }
}
