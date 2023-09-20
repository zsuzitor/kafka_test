using KafkaTestCore.Models.Entity;

namespace KafkaTestCore.Models
{
    public interface IMQHandler
    {
        Task Handle(ConsumerMessage msg);
    }
}
