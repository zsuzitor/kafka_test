using Confluent.Kafka;


namespace KafkaTestCore.Models.Entity
{
    public class KafkaConsumerMessage : ConsumerMessage
    {
        public ConsumeResult<string, string> KafkaResult { get; set; }
    }
}
