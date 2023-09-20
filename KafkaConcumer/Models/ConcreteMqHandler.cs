

using KafkaTestCore.Models;
using KafkaTestCore.Models.Entity;

namespace KafkaConcumer.Models
{
    public class ConcreteMqHandler : IMQHandler
    {
        public async Task Handle(ConsumerMessage msg)
        {
            Console.WriteLine(msg.Value);
        }
    }
}
