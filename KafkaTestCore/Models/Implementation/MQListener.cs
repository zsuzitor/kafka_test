

using Microsoft.Extensions.Logging;

namespace KafkaTestCore.Models.Implementation
{
    public class MQListener : IMQListener
    {
        private readonly IMQConsumer _consumer;
        private readonly IMQHandler _handler;
        private readonly Reconnecter _reconnecter;
        private readonly ILogger _logger;

        public MQListener(IMQConsumer consumer, IMQHandler handler, Reconnecter reconnecter, ILoggerFactory logFactory)
        {
            _consumer = consumer;
            _handler = handler;
            _reconnecter = reconnecter;
            _logger = logFactory.CreateLogger("default");
        }
        public void Dispose()
        {
            _consumer.Dispose();
        }

        public async Task StartListeningAsync(CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                await StartListening(ct);
            }, ct);
        }

        private async Task StartListening(CancellationToken ct)
        {
            _consumer.Connect();
            await _reconnecter.WorkAsync(async () =>
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    var msg = _consumer.Receive(TimeSpan.FromSeconds(2));//todo конфиг
                    if (msg == null)
                    {
                        continue;
                    }

                    var successHandled = await _handler.Handle(msg);
                    if (!successHandled)
                    {
                        _logger.LogWarning($"stoped listening {nameof(MQListener)}");
                        break;
                    }
                    _consumer.MessageWasHandled(msg);
                }
            }, () => _consumer.Reconnect(), ct, 200);//todo time to config
            

        }
    }

}
