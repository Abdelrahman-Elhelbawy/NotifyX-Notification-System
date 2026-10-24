
using NotifyX.Api.Entities;

namespace NotifyX.Api.Services.Outbox;

public interface IOutboxService
{
    // Creates an outbox record for an integration event.
    OutboxMessage CreateRecord(string eventType, string payload);
}
