using StackExchange.Redis;

namespace RedisSortedSet;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private ConnectionMultiplexer _connection;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await ConnectionMultiplexer.ConnectAsync("localhost:6379,password=12345");
        await base.StartAsync(cancellationToken);
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = "1";
        var db = _connection.GetDatabase();
        
        await db.PublishAsync(channel, "2");
        await db.PublishAsync(channel, "3");
        await db.PublishAsync(channel, "4");
        await db.PublishAsync(channel, "5");
        
        var sub = _connection.GetSubscriber();
        await sub.SubscribeAsync(channel, (c, v) =>
        {   
            Console.WriteLine(v);
        });
    }
}