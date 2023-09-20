// See https://aka.ms/new-console-template for more information
using KafkaTestCore.Models;
using KafkaTestCore.Models.Implementation;

Console.WriteLine("Start Produce");


string server = "localhost:29092";//todo config
string kafkaTopic = "topic1test";

var kafkaSettings = new KafkaProducer.Settings() { Server = server };


using IBaseProducer producer = new KafkaToBaseProducer(new KafkaProducer(kafkaSettings), kafkaTopic);
var ctSource = new CancellationTokenSource();

try
{
    for (int i = 0; i < 10; i++)
    {
        await producer.Send(null, $"msg1_{i}", ctSource.Token);

    }
}
catch when (ctSource.Token.IsCancellationRequested)
{
    //stop
}
catch (Exception e)
{
    //todo log
}

Console.WriteLine("End Produce");