// See https://aka.ms/new-console-template for more information
using KafkaTestCore.Models;
using KafkaTestCore.Models.Implementation;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Start Produce");



IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //.AddEnvironmentVariables()
    //.AddCommandLine(args)
    .Build();


string server = Configuration["server"];
string kafkaTopic = Configuration["topic"];

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