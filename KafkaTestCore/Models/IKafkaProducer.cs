

namespace KafkaTestCore.Models
{
    public interface IKafkaProducer : IDisposable
    {
        Task Send(string topic, string? key, string message, CancellationToken ct);
    }
}
