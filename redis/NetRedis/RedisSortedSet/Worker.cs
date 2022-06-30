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
        IDatabase db = _connection.GetDatabase();
        
        var key = "hackers";
        db.KeyDelete(key);
    }

    private static async Task Lexicographically(IDatabase db, string key)
    {
        await db.SortedSetAddAsync(key, "Andrew", 0);
        await db.SortedSetAddAsync(key, "Viktor", 0);
        await db.SortedSetAddAsync(key, "Tester", 0);
        await db.SortedSetAddAsync(key, "Tester2", 0);

        var found = await db.SortedSetRangeByValueAsync(key, "[A", "[Z");
        foreach (var item in found)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine();
    }

    private static async Task SimpleOperations(IDatabase db, string key)
    {
        await db.SortedSetAddAsync(key, "Andrew", 1940);
        await db.SortedSetAddAsync(key, "Viktor", 1955);
        await db.SortedSetAddAsync(key, "Tester", 1966);
        await db.SortedSetAddAsync(key, "Tester2", 1977);

        var found = await db.SortedSetRangeByRankWithScoresAsync(key, 0, -1, Order.Descending);
        foreach (var item in found)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine();

        var foundByScore = await db.SortedSetRangeByScoreWithScoresAsync(key, 1950);
        foreach (var item in foundByScore)
        {
            Console.WriteLine(item);
        }
    }
}