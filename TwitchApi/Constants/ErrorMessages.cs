namespace TwitchAPI.Constants;

public static class ErrorMessages
{
    public const string InvalidOrMissingId = "Invalid or missing 'id' parameter.";
    public const string Unauthorized = "Unauthorized. Twitch access token is invalid or has expired.";
    public const string UserNotFound = "User not found.";
    public const string InternalServerError = "Internal server error.";
    public const string FailedToRetrieveAccessToken = "Failed to retrieve Twitch access token.";
}
