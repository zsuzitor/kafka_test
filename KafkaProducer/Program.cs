// See https://aka.ms/new-console-template for more information
using KafkaTestCore.Models;
using KafkaTestCore.Models.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("Start Produce");



IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //.AddEnvironmentVariables()
    //.AddCommandLine(args)
    .Build();

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
            .AddScoped<Reconnecter>()
            .BuildServiceProvider();


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