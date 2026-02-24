using System.Net;
using System.Text.Json;

namespace TwitchAPI.Tests.Integration;

public sealed class TwitchApiMockHandler : DelegatingHandler
{
    private readonly Dictionary<string, (HttpStatusCode StatusCode, object? Body)> _responses = new();

    public void SetupResponse(string urlContains, HttpStatusCode statusCode, object? body = null)
    {
        _responses[urlContains] = (statusCode, body);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? string.Empty;

        foreach (var (key, (statusCode, body)) in _responses)
        {
            if (!url.Contains(key))
                continue;

            var content = body is not null
                ? JsonSerializer.Serialize(body)
                : string.Empty;

            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            });
        }

        // Default: 200 OK with empty body
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        });
    }
}
