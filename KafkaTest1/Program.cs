using KafkaLib;
using System;
using System.Threading;

namespace KafkaTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = new MessageBus("localhost:29092");
            var tkns = new CancellationTokenSource();

            bus.SendMessage("testTopic1", "message1").Wait(tkns.Token);
            bus.SendMessage("testTopic1", "message1").Wait(tkns.Token);

            Console.ReadKey();
        }
    }
}
