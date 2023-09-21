

namespace KafkaTestCore.Models
{
    public interface IMQListener : IDisposable
    {
        Task StartListeningAsync(CancellationToken ct);
    }
}
