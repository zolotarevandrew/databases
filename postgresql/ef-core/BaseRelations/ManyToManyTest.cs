using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseRelations.ManyToMany;
using BaseRelations.OneToMany;
using BaseRelations.OneToOne;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BaseRelations;

public class ManyToManyTest : TestBase
{
    [Fact]
    public async Task AddUserBook_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        var book = new Book("test");
        var user = new User("user");
        var userBook = new UserBooks(user, book);

        context.Books.Add(book);
        context.Users.Add(user);
        context.UserBooks.Add(userBook);
        await context.SaveChangesAsync();
    }
    
    [Fact]
    public async Task Book_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        var book = await context.Books
            .Include( c => c.UserBooks)
            .ThenInclude( c => c.User)
            .FirstOrDefaultAsync(c => c.Title == "test");
        Assert.NotNull(book);
    }
}