using StackExchange.Redis;

namespace RedisSet;

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
        var store = new TagHashStore(db);
        
        await store.AddAsync("1", new List<Tag>
        {
            new Tag("1", "tag1"),
            new Tag("2", "tag2"),
            new Tag("3", "tag3"),
            new Tag("4", "tag4"),
        });
        var tags = await store.GetAsync("1");
    }
}