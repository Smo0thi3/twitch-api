using System.Text.Json.Serialization;

namespace TwitchAPI.Models.Responses;

public sealed class StreamResponse
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("user_name")]
    public string UserName { get; init; } = string.Empty;
}

public sealed class TwitchStreamsResponse
{
    [JsonPropertyName("data")]
    public List<StreamResponse> Data { get; init; } = [];
}