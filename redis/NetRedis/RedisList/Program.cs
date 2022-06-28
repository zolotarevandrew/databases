using RedisList;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Publisher>();
        services.AddHostedService<Subscriber>();
    })
    .Build();

await host.RunAsync();