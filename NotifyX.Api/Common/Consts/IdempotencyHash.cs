using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Services;

public static class IdempotencyHash
{
    public static string Compute(CreateNotificationRequest request)
    {
        var canonicalRequest = new
        {
            request.Title,
            request.Body,
            request.Type,
            request.Priority,
            request.Metadata,

            UserIds = request.UserIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray()
        };

        var json = JsonSerializer.Serialize(canonicalRequest);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}