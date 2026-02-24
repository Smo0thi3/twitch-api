using System.Text.Json.Serialization;

namespace TwitchAPI.Models.Responses;

public sealed class TwitchTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}
