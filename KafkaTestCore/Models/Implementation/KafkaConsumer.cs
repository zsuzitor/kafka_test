using Confluent.Kafka;
using KafkaTestCore.Models.Entity;

namespace KafkaTestCore.Models.Implementation
{

    public class KafkaConsumer : IMQConsumer
    {
        private IConsumer<string, string> _kafkaConsumer;
        private readonly Settings _settings;

        public class Settings
        {
            public string Server { get; set; }
            public string Topic { get; set; }
            public string GroupId { get; set; }
        }

        public KafkaConsumer(Settings settings)
        {
            _settings = settings;
        }

        public ConsumerMessage Receive(TimeSpan timeout)
        {
            try
            {
                var cr = _kafkaConsumer.Consume(timeout);
                if (cr?.Message == null)
                {
                    return null;
                }

                return new KafkaConsumerMessage() { Key = cr.Key, Value = cr.Message.Value, KafkaResult = cr };
            }
            catch (Exception ex)
            {
                throw;//todo тут исключение расконнекта
            }

        }

        public void MessageWasHandled(ConsumerMessage msg)
        {
            try
            {
                var kafkaMessage = msg as KafkaConsumerMessage;
                _kafkaConsumer.Commit(kafkaMessage.KafkaResult);
            }
            catch (Exception ex)
            {
                throw;//todo тут исключение расконнекта
            }

        }
        public void Reconnect()
        {
            Disconnect();
            Connect();
        }
        public void Connect()
        {
            try
            {
                if (_kafkaConsumer == null)
                {
                    var cConfig = new ConsumerConfig
                    {
                        BootstrapServers = _settings.Server,
                        GroupId = _settings.GroupId,
                        EnableAutoCommit = false,
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                    };
                    string topic = _settings.Topic;
                    _kafkaConsumer = new ConsumerBuilder<string, string>(cConfig).Build();
                    _kafkaConsumer.Subscribe(topic);
                }

            }
            catch (Exception ex)
            {
                throw;//todo тут исключение расконнекта
            }


        }

        //public async Task StartListening(Settings settings, CancellationToken ct)
        //{
        //    var cConfig = new ConsumerConfig
        //    {
        //        BootstrapServers = settings.Server,
        //        GroupId = settings.GroupId,
        //        EnableAutoCommit = false
        //    };
        //    string topic = settings.Topic;

        //    using (var consumer = new ConsumerBuilder<string, int>(cConfig).Build())
        //    {
        //        //consumer.Assign - можно выбрать топик и офсет
        //        consumer.Subscribe(topic);
        //        while (true)
        //        {
        //            ct.ThrowIfCancellationRequested();
        //            var cr = consumer.Consume(ct);
        //            //todo handle
        //            consumer.Commit(cr);

        //            //cr.Message.Key, cr.Message.Value
        //        }


        //    }

        //}

        public void Disconnect()
        {

            _kafkaConsumer?.Unsubscribe();
            _kafkaConsumer?.Close();
            _kafkaConsumer?.Dispose();
            _kafkaConsumer = null;

        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
