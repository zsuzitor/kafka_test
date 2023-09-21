// See https://aka.ms/new-console-template for more information
using KafkaConcumer.Models;
using KafkaTestCore.Models.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");


IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //.AddEnvironmentVariables()
    //.AddCommandLine(args)
    .Build();


string server = Configuration["server"];
string kafkaTopic = Configuration["topic"];
string groupId = Configuration["groupId"];


var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddFilter("Microsoft", LogLevel.Information)
           .AddFilter("System", LogLevel.Information)
           .AddFilter("SampleApp.Program", LogLevel.Information)//todo SampleApp.Program????
           .AddConsole();
});


var ctSource = new CancellationTokenSource();
using var listener = new MQListener(
    new KafkaConsumer(new KafkaConsumer.Settings() { Server = server, Topic = kafkaTopic, GroupId = groupId })
    , new ConcreteMqHandler(), new KafkaTestCore.Models.Reconnecter(loggerFactory), loggerFactory);
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