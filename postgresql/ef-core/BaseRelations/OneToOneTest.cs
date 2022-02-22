using System.Threading.Tasks;
using BaseRelations.OneToOne;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BaseRelations;

public class OneToOneTest : TestBase
{
    [Fact]
    public async Task AddUser_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        context.Users.Add(new User("test"));
        await context.SaveChangesAsync();

        var user = await context.Users.FirstOrDefaultAsync(c => c.UserName == "test");
        Assert.NotNull(user);

        context.Remove(user);
        await context.SaveChangesAsync();
        
        user = await context.Users.FirstOrDefaultAsync(c => c.UserName == "test");
        Assert.Null(user);
    }
    
    [Fact]
    public async Task AddAddress_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        context.Users.Add(new User("test"));
        await context.SaveChangesAsync();

        var user = await context.Users.FirstOrDefaultAsync(c => c.UserName == "test");
        Assert.NotNull(user);

        var address = new Address(user, "street");
        await context.Addresses.AddAsync(address);
        await context.SaveChangesAsync();
        
        address = await context.Addresses.FirstOrDefaultAsync(c => c.Street == "street");
        Assert.NotNull(address);
        Assert.NotNull(address.User);
        Assert.True(address.UserId > 0);
        
        user = await context.Users.FirstOrDefaultAsync(c => c.UserName == "test");
        Assert.NotNull(user.Address);

        context.Remove(user);
        context.Remove(address);
        await context.SaveChangesAsync();
        
        user = await context.Users.FirstOrDefaultAsync(c => c.UserName == "test");
        Assert.Null(user);
        
        address = await context.Addresses.FirstOrDefaultAsync(c => c.Street == "street");
        Assert.Null(address);
    }
}