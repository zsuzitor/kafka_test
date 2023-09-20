
namespace KafkaTestCore.Models
{
    public interface IBaseProducer : IDisposable
    {
        Task Send(string? key, string message, CancellationToken ct);
    }
}
