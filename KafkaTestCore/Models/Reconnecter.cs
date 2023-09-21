using Microsoft.Extensions.Logging;
using static KafkaTestCore.Models.ActionStep;

namespace KafkaTestCore.Models
{
    public class Reconnecter
    {
        private readonly ILogger _logger;
        public Reconnecter(ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger("default");
        }

        public async Task WorkAsync(Func<Task> act, Action actReconnect, CancellationToken stoppingToken, int timeToReconnect)
        {
            try
            {
                //первый раз просто дергаем метод, если отвалится уже будем подрубать логику реконнекта
                await act();
            }
            catch (ErrorWithConnectException e)
            {
                IReconnectStep step = new ActionStep(new ReconnectorContext()
                {
                    Act = act,
                    ActReconnect = actReconnect,
                    PreviousStep = ReconnectStepEnum.None,
                    StoppingToken = stoppingToken,
                    TimeToReconnect = timeToReconnect,
                    Logger = _logger
                });

                while (step != null)
                {
                    step = await step.StepAsync();
                }
            }

        }

        public void Work(Action act, Action actReconnect, CancellationToken stoppingToken, int timeToReconnect)
        {
            try
            {
                //первый раз просто дергаем метод, если отвалится уже будем подрубать логику реконнекта
                act();
            }
            catch (ErrorWithConnectException e)
            {
                IReconnectStep step = new ActionStep(new ReconnectorContext()
                {
                    Act = () => { act(); return Task.CompletedTask; },
                    ActReconnect = actReconnect,
                    PreviousStep = ReconnectStepEnum.None,
                    StoppingToken = stoppingToken,
                    TimeToReconnect = timeToReconnect,
                    Logger = _logger
                });

                while (step != null)
                {
                    step = step.StepAsync().GetAwaiter().GetResult();
                }
            }

        }
    }

    public enum ReconnectStepEnum { None, Action, Reconnect, Wait }

    public class ReconnectorContext
    {
        public Func<Task> Act { get; set; }
        public Action ActReconnect { get; set; }
        public CancellationToken StoppingToken { get; set; }
        public int TimeToReconnect { get; set; }
        public ReconnectStepEnum PreviousStep { get; set; }
        public ILogger Logger { get; set; }
    }

    public interface IReconnectStep
    {
        Task<IReconnectStep> StepAsync();
    }

    public class ActionStep : IReconnectStep
    {
        private readonly ReconnectorContext _ctx;
        public ActionStep(ReconnectorContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IReconnectStep> StepAsync()
        {
            if (_ctx.StoppingToken.IsCancellationRequested)
            {
                return null;
            }

            var prev = _ctx.PreviousStep;
            _ctx.PreviousStep = ReconnectStepEnum.Action;
            try
            {
                await _ctx.Act();
                return null;
            }
            catch (ErrorWithConnectException e)
            {
                _ctx.Logger.LogError("Ошибка подключения", e);
                if (prev == ReconnectStepEnum.Reconnect)
                {
                    return new WaitStep(_ctx);
                }

                return new ReconnectStep(_ctx);
            }
        }

        public class WaitStep : IReconnectStep
        {
            private readonly ReconnectorContext _ctx;
            public WaitStep(ReconnectorContext ctx)
            {
                _ctx = ctx;
            }

            public async Task<IReconnectStep> StepAsync()
            {
                if (_ctx.StoppingToken.IsCancellationRequested)
                {
                    return null;
                }

                //var prev = _ctx.PreviousStep;

                await Task.Delay(_ctx.TimeToReconnect);
                _ctx.PreviousStep = ReconnectStepEnum.Wait;
                return new ReconnectStep(_ctx);
            }
        }

        public class ReconnectStep : IReconnectStep
        {
            private readonly ReconnectorContext _ctx;
            public ReconnectStep(ReconnectorContext ctx)
            {
                _ctx = ctx;
            }

            public async Task<IReconnectStep> StepAsync()
            {
                if (_ctx.StoppingToken.IsCancellationRequested)
                {
                    return null;
                }

                //var prev = _ctx.PreviousStep;

                _ctx.PreviousStep = ReconnectStepEnum.Reconnect;
                try
                {
                    _ctx.ActReconnect();
                    return new ActionStep(_ctx);
                }
                catch (ErrorWithConnectException e)
                {
                    _ctx.Logger.LogError("Ошибка реконнекта", e);
                    return new WaitStep(_ctx);
                }
            }


        }

        public class ErrorWithConnectException : Exception
        {

        }
    }
}
