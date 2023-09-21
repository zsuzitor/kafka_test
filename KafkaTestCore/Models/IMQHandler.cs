using KafkaTestCore.Models.Entity;

namespace KafkaTestCore.Models
{
    public interface IMQHandler
    {
        Task<bool> Handle(ConsumerMessage msg);
    }
}
