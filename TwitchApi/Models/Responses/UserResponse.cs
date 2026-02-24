using System.Text.Json.Serialization;

namespace TwitchAPI.Models.Responses;

public sealed class UserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("login")]
    public string Login { get; init; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("broadcaster_type")]
    public string BroadcasterType { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("profile_image_url")]
    public string ProfileImageUrl { get; init; } = string.Empty;

    [JsonPropertyName("offline_image_url")]
    public string OfflineImageUrl { get; init; } = string.Empty;

    [JsonPropertyName("view_count")]
    public int ViewCount { get; init; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; init; } = string.Empty;
}

public sealed class TwitchUsersResponse
{
    [JsonPropertyName("data")]
    public List<UserResponse> Data { get; init; } = [];
}