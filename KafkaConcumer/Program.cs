// See https://aka.ms/new-console-template for more information
using KafkaConcumer.Models;
using KafkaTestCore.Models.Implementation;

Console.WriteLine("Hello, World!");



string server = "";
string topic = "";
string groupId = "";

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