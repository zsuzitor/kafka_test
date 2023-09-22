using ConsumerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(conf => {
        conf.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);//��� ����� ������� �� ����������
    })
    //.UseWindowsService()//��������� ��� ������ ��� ������
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
