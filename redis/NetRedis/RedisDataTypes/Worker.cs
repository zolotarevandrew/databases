using StackExchange.Redis;

namespace RedisDataTypes;

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
            await HyperLogLog(db);
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    async Task HyperLogLog(IDatabase db)
    {
        await db.KeyDeleteAsync("1");
        await db.HyperLogLogAddAsync("1", "1");
        await db.HyperLogLogAddAsync("1", "2");
        await db.HyperLogLogAddAsync("1", "3");
        var res = await db.HyperLogLogLengthAsync("1");

        Console.WriteLine(res);
    }
    
    async Task BitArrays(IDatabase db)
    {
        await db.KeyDeleteAsync("1");
        await db.StringSetAsync("1", "12345");
        var res = await db.StringSetBitAsync("1", 0, true);

        Console.WriteLine(await db.StringGetAsync("1"));
    }
    
    async Task SortedSets(IDatabase db)
    {
        await db.KeyDeleteAsync("1");
        await db.SortedSetAddAsync("1", new SortedSetEntry[] {new SortedSetEntry("name", 1)});
        var res = await db.SortedSetPopAsync("1");

        Console.WriteLine(res.Value.ToString());
    }
    
    async Task Hashes(IDatabase db)
    {
        await db.KeyDeleteAsync("1");
        await db.HashSetAsync("1", new HashEntry[] {new HashEntry("name", "value")});
        var res = await db.HashGetAllAsync("1");

        Console.WriteLine((await db.SetMembersAsync("1")).Select(c => c.ToString()));
    }
    
    async Task Sets(IDatabase db)
    {
        await db.KeyDeleteAsync("1");
        var res = await db.SetAddAsync("1", "2");
        res = await db.SetAddAsync("1", "2");
        res = await db.SetAddAsync("1", "3");
        
        Console.WriteLine((await db.SetMembersAsync("1")).Select(c => c.ToString()));
    }

    async Task Strings(IDatabase db)
    {
        var res = await db.StringSetAsync("1", "2");
        Console.WriteLine(res);
            
        Console.WriteLine(await db.StringGetAsync("1"));
            
        Console.WriteLine(await db.StringAppendAsync("1", "3"));
            
        Console.WriteLine(await db.StringGetAsync("1"));
        Console.WriteLine(await db.StringLengthAsync("1"));
    }
}