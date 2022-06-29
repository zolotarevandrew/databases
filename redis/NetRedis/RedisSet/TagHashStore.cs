using StackExchange.Redis;

namespace RedisSet;

public record Tag(string Id, string Name);

public class TagHashStore
{
    private readonly IDatabase _db;

    public TagHashStore(IDatabase db)
    {
        _db = db;
    }


    public async Task<IReadOnlyList<Tag>> GetAsync(string articleId)
    {
        var tagNames = await _db.HashGetAllAsync($"article:{articleId}:tagNames");
        if (tagNames.Length == 0) return new List<Tag>();

        return tagNames.Select(c =>
        {
            var tag = new Tag(c.Name.ToString(), c.Value.ToString());
            return tag;
        }).ToList();
    }
    
    public async Task AddAsync(string articleId, List<Tag> tags)
    {
        var tasks = new List<Task>
        {
            _db.HashSetAsync($"article:{articleId}:tagNames", tags.Select(c => new HashEntry(c.Id, c.Name)
            {

            }).ToArray())
        };

        await Task.WhenAll(tasks);
    }
}

public class TagSetStore
{
    private readonly IDatabase _db;

    public TagSetStore(IDatabase db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Tag>> GetAsync(List<string> ids)
    {
        var tagIds = await _db.SetCombineAsync(SetOperation.Intersect, ids.Select(c => new RedisKey(c)).ToArray());
        if (tagIds.Length == 0) return new List<Tag>();
        
        
        return tagIds.Select(c =>
        {
            var tag = new Tag(c.ToString(), "");
            return tag;
        }).ToList();
    }
    
    public async Task AddAsync(List<Tag> tags)
    {
        var tasks = new List<Task>();
        tasks.AddRange(tags.Select(t => _db.SetAddAsync($"tag:{t.Id}:articles", t.Id)));

        await Task.WhenAll(tasks);
    }
}