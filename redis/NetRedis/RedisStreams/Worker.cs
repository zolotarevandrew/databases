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

        var key = "streams:1";
        await db.StreamAddAsync(key, new NameValueEntry[]
        {
            new NameValueEntry("name", "2"),
            new NameValueEntry("age", 2),
            new NameValueEntry("priority", 227),
        });
        await db.StreamAddAsync(key, new NameValueEntry[]
        {
            new NameValueEntry("name", "3"),
            new NameValueEntry("age", 3),
            new NameValueEntry("priority", 228),
        });
        
        var key2 = "streams:2";
        await db.StreamAddAsync(key2, new NameValueEntry[]
        {
            new NameValueEntry("name2", "2"),
            new NameValueEntry("age2", 2),
            new NameValueEntry("priority2", 227),
        });
        await db.StreamAddAsync(key2, new NameValueEntry[]
        {
            new NameValueEntry("name2", "3"),
            new NameValueEntry("age2", 3),
            new NameValueEntry("priority2", 228),
        });

        var range = await db.StreamRangeAsync(key);
        ReadStream(key, range);

        var readed = await db.StreamReadAsync(key, 0);
        ReadStream(key, readed);
        
        var streams = await db.StreamReadAsync(new StreamPosition[]
        {
            new StreamPosition(key, 0),
            new StreamPosition(key2, 0),
            
        });
        foreach (var stream in streams)
        {
            ReadStream(stream.Key, stream.Entries);
        }
    }

    private static void ReadStream(RedisKey key, StreamEntry[] range)
    {
        Console.WriteLine(key);
        Console.WriteLine();
        
        foreach (var item in range)
        {
            foreach (var value in item.Values)
            {
                Console.Write(value.Name + " " + value.Value);
            }

            Console.WriteLine();
        }
    }
}