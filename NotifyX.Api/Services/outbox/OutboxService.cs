
using NotifyX.Api.Entities;

namespace NotifyX.Api.Services.Outbox;

public sealed class OutboxService : IOutboxService
{
    /**
     * Creates a new outbox message record representing an event that should be persisted
     * and processed by the background outbox worker.
     *
     * @param eventType The type or name of the event being recorded.
     * @param payload The serialized event payload to store for later delivery.
     * @returns An initialized OutboxMessage with a unique identifier and timestamp.
     */
    public OutboxMessage CreateRecord(string eventType, string payload){
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Payload = payload,
            Attempts = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
