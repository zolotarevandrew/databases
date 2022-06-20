using StackExchange.Redis;

namespace RedisCaching;

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
        IDatabase db = _connection.GetDatabase();
        while (!stoppingToken.IsCancellationRequested)
        {
            string value = "abcdefg";
            db.StringSet("mykey", value);
            
            value = db.StringGet("mykey");
            
            Console.WriteLine(value);
            await Task.Delay(1000, stoppingToken);
        }
    }
}