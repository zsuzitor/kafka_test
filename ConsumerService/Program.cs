using ConsumerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(conf => {
        conf.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);//без этого конфиги не прорастают
    })
    //.UseWindowsService()//указываем что хостим под виндой
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        //logging.ClearProviders();
        //logging.SetMinimumLevel(LogLevel.Information);
        //logging.AddNLog();
    })
    .Build();

host.Run();
