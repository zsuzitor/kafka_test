using System;
using System.Threading;
using KafkaLib;

namespace MicroService
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = new MessageBus("localhost:29092");
            var tkns = new CancellationTokenSource();

            bus.StartConsume("testTopic1", tkns.Token, (s) =>
            {
                Console.WriteLine(s);
            });

            Console.ReadKey();
        }
    }
}
