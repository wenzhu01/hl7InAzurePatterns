using mllpServer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMllpServer, MllpServer>();
//services.AddApplicationInsightsTelemetry(); 
// Add this line of code to enable Application Insights.services.AddServiceProfiler(); 
// Add this line of code to enable Profiler
//services.AddServiceProfiler(); // Add this line of code to enable Profiler

        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();
