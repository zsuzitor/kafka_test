

using KafkaTestCore.Models;
using KafkaTestCore.Models.Entity;

namespace KafkaConcumer.Models
{
    public class ConcreteMqHandler : IMQHandler
    {
        public async Task Handle(ConsumerMessage msg)
        {
            var kmsg = msg as KafkaConsumerMessage;
            Console.WriteLine($"received-{msg.Value}-{kmsg.KafkaResult.Partition.Value}-{kmsg.KafkaResult.Offset}");
        }
    }
}
