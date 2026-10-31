
using Microsoft.AspNetCore.SignalR;

namespace NotifyX.Api.Hubs;

public sealed class QueryStringUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var userId = connection.GetHttpContext()?
            .Request.Query["userId"]
            .ToString();

        return string.IsNullOrWhiteSpace(userId)
            ? null
            : userId;
    }
}
