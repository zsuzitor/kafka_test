

using Microsoft.Extensions.Logging;

namespace KafkaTestCore.Models.Implementation
{
    public class MQListener : IMQListener
    {
        public class Settings
        {
            public int ReceiveTimeout { get; set; }
            public int ReconnecterTimeout { get; set; }
        }

        private readonly IMQConsumer _consumer;
        private readonly IMQHandler _handler;
        private readonly Reconnecter _reconnecter;
        private readonly ILogger _logger;
        private readonly Settings _settings;

        public MQListener(IMQConsumer consumer, IMQHandler handler, Reconnecter reconnecter, ILoggerFactory logFactory, Settings settings)
        {
            _consumer = consumer;
            _handler = handler;
            _reconnecter = reconnecter;
            _logger = logFactory.CreateLogger("default");
            _settings = settings;
        }
        public void Dispose()
        {
            _consumer.Dispose();
        }


        public async Task StartListeningAsync(CancellationToken ct)
        {
            try
            {
                await _reconnecter.WorkAsync(async () =>
                {
                    _consumer.Connect();
                    while (true)
                    {
                        ct.ThrowIfCancellationRequested();

                        var msg = _consumer.Receive(TimeSpan.FromSeconds(_settings.ReceiveTimeout));//todo конфиг
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
                }, () => _consumer.Reconnect(), ct, _settings.ReconnecterTimeout);//todo time to config

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }
    }

}
