using Confluent.Kafka;
using KafkaTestCore.Models.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using static KafkaTestCore.Models.ActionStep;

namespace KafkaTestCore.Models.Implementation
{

    public class KafkaConsumer : IMQConsumer
    {
        public class Settings
        {
            public string Server { get; set; }
            public string Topic { get; set; }
            public string GroupId { get; set; }
        }

        private IConsumer<string, string> _kafkaConsumer;
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public KafkaConsumer(Settings settings, ILoggerFactory logFactory)
        {
            _settings = settings;
            _logger = logFactory.CreateLogger("default");
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

                _logger.LogInformation($"{nameof(KafkaConsumer)}-concumed-topic:{_settings.Topic}-offset:{cr.Offset}-partition:{cr.Partition}");
                return new KafkaConsumerMessage() { Key = cr.Key, Value = cr.Message.Value, KafkaResult = cr };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ErrorWithConnectException();
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
                _logger.LogError(ex, ex.Message);
                throw new ErrorWithConnectException();
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
                    _logger.LogInformation($"{nameof(KafkaConsumer)}-try to connect-topic:{_settings.Topic}");
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
                    _logger.LogInformation($"{nameof(KafkaConsumer)}-connected-topic:{_settings.Topic}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ErrorWithConnectException();
            }


        }

        

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
