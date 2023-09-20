

namespace KafkaTestCore.Models.Implementation
{
    public class MQListener : IMQListener
    {
        private readonly IMQConsumer _consumer;
        private readonly IMQHandler _handler;

        public MQListener(IMQConsumer consumer, IMQHandler handler)
        {
            _consumer = consumer;
            _handler = handler;
        }
        public void Dispose()
        {
            _consumer.Dispose();
        }

        public async Task StartListeningAsync(CancellationToken ct)
        {
            await Task.Run(() =>
            {
                StartListening(ct);
            }, ct);
        }

        public void StartListening(CancellationToken ct)
        {
            _consumer.Connect();
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var msg = _consumer.Receive(TimeSpan.FromSeconds(2));//todo конфиг
                if (msg == null)
                {
                    continue;
                }
                //todo handle
                _handler.Handle(msg);
                _consumer.MessageWasHandled(msg);


            }

        }
    }

}
