using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaLib
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public sealed class MessageBus // : IDisposable
    {
        //private readonly Producer<Null, string> _producer;
        //private Consumer<Null, string> _consumer;

        private string _host;

        //private readonly IDictionary<string, object> _producerConfig;
        //private readonly IDictionary<string, object> _consumerConfig;

        public MessageBus() : this("localhost")
        {
        }

        public MessageBus(string host)
        {
            _host = host;
            //_producerConfig = new Dictionary<string, object> { { "bootstrap.servers", host } };
            //_consumerConfig = new Dictionary<string, object>
            //{
            //    { "group.id", "custom-group"},
            //    { "bootstrap.servers", host }
            //};

            //_producer = new Producer<Null, string>(_producerConfig, null, new StringSerializer(Encoding.UTF8));
        }

        public async Task SendMessage(string topic, string message)
        {
            //_producer.ProduceAsync(topic, null, message);

            var config = new ProducerConfig
            {
                BootstrapServers = _host, //"host1:9092,host2:9092",
                ClientId = Dns.GetHostName(),
            };


            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var t = producer.ProduceAsync(topic, new Message<Null, string> {Value = message});
                await t.ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                    }
                    else
                    {
                        Console.WriteLine($"Wrote to offset: {task.Result.Offset}");
                    }
                });

                //но вот так будет быстрее чем с async 
                //producer.Produce(
                //    "my-topic", new Message<Null, string> { Value = "hello world" }, handler);
            }
        }


        public void StartConsume(string topic, CancellationToken cancellationToken, Action<string> handler)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _host,
                GroupId = "foo1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                // Disable auto-committing of offsets.
                //EnableAutoCommit = false,
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(new[] {topic});

                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    handler(consumeResult.Message.Value);

                    //можно явно комитать офсет, но вроде как лучше так не делать
                    //if (consumeResult.Offset % commitPeriod == 0)
                    //{
                    //    try
                    //    {
                    //        consumer.Commit(consumeResult);
                    //    }
                    //    catch (KafkaException e)
                    //    {
                    //        Console.WriteLine($"Commit error: {e.Error.Reason}");
                    //    }
                    //}
                }

                consumer.Close();
            }
        }


    }
}
