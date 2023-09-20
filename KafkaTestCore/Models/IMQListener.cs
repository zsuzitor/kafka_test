

namespace KafkaTestCore.Models
{
    public interface IMQListener : IDisposable
    {
        Task StartListeningAsync(CancellationToken ct);
        void StartListening(CancellationToken ct);
    }
}
