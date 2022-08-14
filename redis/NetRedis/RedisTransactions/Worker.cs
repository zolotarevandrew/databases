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
db.set
        await ParallelSet(db);
    }

    private static async Task ParallelSet(IDatabase db)
    {
        var run = TestTran(db);
        var run2 = TestTran(db);
        var run3 = TestTran(db);
        await Task.WhenAll(run, run2, run3);
    }

    private static async Task TestTran(IDatabase db)
    {
        var custKey = "tran:testkey1";
        var newId = "TestId3";
        var tran = db.CreateTransaction();
        tran.AddCondition(Condition.HashNotExists(custKey, "UniqueID"));
        //we shouldn't wait there
        var task = tran.HashSetAsync(custKey, "UniqueID", newId);
        bool committed = await tran.ExecuteAsync();
        Console.WriteLine(committed);
    }
}