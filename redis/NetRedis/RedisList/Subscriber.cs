using System.Text.Json;
using StackExchange.Redis;

namespace RedisList;

public class Subscriber : BackgroundService
{
    private readonly ILogger<Subscriber> _logger;
    private ConnectionMultiplexer _connection;

    public Subscriber(ILogger<Subscriber> logger)
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
        await _connection.GetSubscriber().UnsubscribeAsync(Queue.Name);
        
        await _connection.CloseAsync();
        
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IDatabase db = _connection.GetDatabase();
        var sub = _connection.GetSubscriber();
        
        
        await sub.SubscribeAsync(Queue.Name, (_, _) =>
        {
            var val = db.ListRightPop(Queue.Name);
            var str = val.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                var msg = JsonSerializer.Deserialize<Message>(str);
                Console.WriteLine("Consumer" + msg.Test);    
            }
        });
    }
}