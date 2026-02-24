namespace TwitchAPI.Models.Responses;

public sealed class ErrorResponse
{
    public string Error { get; init; } = string.Empty;

    public static ErrorResponse From(string message) => new() { Error = message };
}
