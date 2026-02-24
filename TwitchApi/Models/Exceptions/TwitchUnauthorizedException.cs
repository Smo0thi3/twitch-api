namespace TwitchAPI.Models.Exceptions;

public sealed class TwitchUnauthorizedException : Exception
{
    public TwitchUnauthorizedException() : base("Twitch access token is invalid or has expired.") { }
}
