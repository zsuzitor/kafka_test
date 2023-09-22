// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using KafkaConcumer.Models;
using KafkaTestCore.Models;
using KafkaTestCore.Models.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");


IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //.AddEnvironmentVariables()
    //.AddCommandLine(args)
    .Build();


string server = Configuration["Server"];
string kafkaTopic = Configuration["Topic"];
string groupId = Configuration["GroupId"];
string receiveTimeout = Configuration["ReceiveTimeout"];
string reconnecterTimeout = Configuration["ReconnecterTimeout"];


var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddFilter("Microsoft", LogLevel.Information)
           .AddFilter("System", LogLevel.Information)
           .AddFilter("SampleApp.Program", LogLevel.Information)//todo SampleApp.Program????
           .AddConsole();
});


var serviceProvider = new ServiceCollection()
            //.AddLogging()
            .AddSingleton<ILoggerFactory>((_) => loggerFactory)
            .AddSingleton<IConfiguration>((_) => Configuration)
            .BuildServiceProvider();
ServiceLocator.Init(serviceProvider);






var logger = loggerFactory.CreateLogger("default");
var ctSource = new CancellationTokenSource();
using var listener = new MQListener(
    new KafkaConsumer(new KafkaConsumer.Settings() { Server = server, Topic = kafkaTopic, GroupId = groupId }, loggerFactory)
    , new ConcreteMqHandler(), new KafkaTestCore.Models.Reconnecter(loggerFactory), loggerFactory
    , new MQListener.Settings() { });
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
    Console.WriteLine("завершено с ошибкой");
    Console.WriteLine(e.Message);
}


Console.WriteLine("End Consume");