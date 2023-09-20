using KafkaTestCore.Models.Entity;

namespace KafkaTestCore.Models
{
    public interface IMQConsumer : IDisposable
    {
        void Connect();
        ConsumerMessage Receive(TimeSpan timeout);
        void MessageWasHandled(ConsumerMessage msg);
    }
}
