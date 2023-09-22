using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace KafkaTestCore.Models.Implementation
{
    //возможно обернуть это еще в producerBase
    public class KafkaProducer : IKafkaProducer
    {
        public class Settings
        {
            public string Server { get; set; }
        }


        //создание и удаление топиков из шарпа
        //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/ExactlyOnce/Program.cs

        private IProducer<string, string> _producer;
        private readonly ILogger _logger;

        public KafkaProducer(Settings settings, ILoggerFactory logFactory)
        {

            var config = new ProducerConfig
            {
                BootstrapServers = settings.Server,

            };

            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logFactory.CreateLogger("default");
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
            _producer = null;
        }

        //public async Task Send(string topic, string message, CancellationToken ct)
        //{
        //    var msg = new Message<Null, string> { Value = message };
        //    _ = await _producer.ProduceAsync(topic,msg, ct);
        //}

        public async Task Send(string topic, string? key, string message, CancellationToken ct)
        {
            try
            {
                var msg = new Message<string, string> { Value = message, Key = key };
                _ = await _producer.ProduceAsync(topic, msg, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
