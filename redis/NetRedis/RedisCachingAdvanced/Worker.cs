using StackExchange.Redis;

namespace RedisCachingAdvanced;

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

        var key = await SmartSetAsync(db);
        while (!stoppingToken.IsCancellationRequested)
        {
            var res = await db.StringGetAsync(key);
            Console.WriteLine(res);
            await Task.Delay(1000, stoppingToken);
        }
    }

    async Task<string> SmartSetAsync(IDatabase db)
    {
        string key = "mykey";
        var result = await db.StringSetAsync(
            key,
            "test",
            TimeSpan.FromSeconds(10),
            keepTtl: true,
            When.NotExists,
            CommandFlags.FireAndForget);
        Console.WriteLine(result);
        return key;
    }
}