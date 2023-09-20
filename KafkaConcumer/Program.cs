// See https://aka.ms/new-console-template for more information
using KafkaConcumer.Models;
using KafkaTestCore.Models.Implementation;

Console.WriteLine("Hello, World!");



string server = "localhost:29092";
string topic = "topic1test";
string groupId = "group1";

var ctSource = new CancellationTokenSource();
var listener = new MQListener(
    new KafkaConsumer(new KafkaConsumer.Settings() { Server = server, Topic = topic, GroupId = groupId })
    , new ConcreteMqHandler());
try
{
    
    await listener.StartListeningAsync(ctSource.Token);
}
catch when (ctSource.Token.IsCancellationRequested)
{
    //stop
}
catch (Exception e)
{
    //todo log
}