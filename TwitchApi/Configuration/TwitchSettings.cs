namespace TwitchAPI.Configuration;

public sealed class TwitchSettings
{
    public const string SectionName = "Twitch";

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string AuthUrl { get; init; } = string.Empty;
    public string ApiBaseUrl { get; init; } = string.Empty;
}
