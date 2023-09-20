using Confluent.Kafka;

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

        public KafkaProducer(Settings settings)
        {

            var config = new ProducerConfig
            {
                BootstrapServers = settings.Server,

            };

            _producer = new ProducerBuilder<string, string>(config).Build();
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
            var msg = new Message<string, string> { Value = message, Key = key };
            _ = await _producer.ProduceAsync(topic, msg, ct);
        }
    }

}
