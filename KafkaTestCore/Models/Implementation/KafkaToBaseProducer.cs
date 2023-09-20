

namespace KafkaTestCore.Models.Implementation
{
    public class KafkaToBaseProducer : IBaseProducer
    {
        private readonly IKafkaProducer _producer;
        private readonly string _topic;
        public KafkaToBaseProducer(IKafkaProducer producer, string topic)
        {
            _producer = producer;
            _topic = topic;
        }

        public void Dispose()
        {
            _producer.Dispose();
        }

        public async Task Send(string? key, string message, CancellationToken ct)
        {
            await _producer.Send(_topic, key, message, ct);
        }
    }
}
