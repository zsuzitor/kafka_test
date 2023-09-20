// See https://aka.ms/new-console-template for more information
using KafkaTestCore.Models;
using KafkaTestCore.Models.Implementation;

Console.WriteLine("Start Produce");


string server = "";//todo config
string kafkaTopic = "test_topic_1";

var kafkaSettings = new KafkaProducer.Settings() { Server = server };


using IBaseProducer producer = new KafkaToBaseProducer(new KafkaProducer(kafkaSettings), kafkaTopic);
var ctSource = new CancellationTokenSource();

try
{
    await producer.Send( null, "msg1", ctSource.Token);
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