using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace RedisList;

public class Publisher : BackgroundService
{
    private readonly ILogger<Publisher> _logger;
    private ConnectionMultiplexer _connection;

    public Publisher(ILogger<Publisher> logger)
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
        IDatabase db = _connection.GetDatabase();
        var sub = _connection.GetSubscriber();
        int idx = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var msg = new Message
            {
                Test = idx
            };
            db.ListLeftPush(Queue.Name, new RedisValue(JsonSerializer.Serialize(msg)), flags: CommandFlags.FireAndForget);
            await sub.PublishAsync(Queue.Name, "");
            await Task.Delay(1000, stoppingToken);
            idx++;
        }
    }
}