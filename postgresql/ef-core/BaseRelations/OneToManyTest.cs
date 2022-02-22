using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseRelations.OneToMany;
using BaseRelations.OneToOne;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BaseRelations;

public class OneToManyTest : TestBase
{
    [Fact]
    public async Task AddBook_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        context.Books.Add(new Book("test"));
        await context.SaveChangesAsync();

        var book = await context.Books.FirstOrDefaultAsync(c => c.Title == "test");
        Assert.NotNull(book);

        context.Remove(book);
        await context.SaveChangesAsync();
        
        book = await context.Books.FirstOrDefaultAsync(c => c.Title == "test");
        Assert.Null(book);
    }
    
    [Fact]
    public async Task AddBookAndReviews_ShouldBeSuccess()
    {
        using var context = _sp.GetRequiredService<AppContext>();

        context.Books.Add(new Book("test"));
        await context.SaveChangesAsync();

        var book = await context.Books.FirstOrDefaultAsync(c => c.Title == "test");
        Assert.NotNull(book);

        var review1 = new Review(book, "review1");
        var review2 = new Review(book, "review2");
        var review3 = new Review(book, "review3");

        await context.AddRangeAsync(new List<Review>
        {
            review1,
            review2,
            review3
        });
        await context.SaveChangesAsync();
        
        review1 = await context.Reviews.FirstOrDefaultAsync(c => c.Content == "review1");
        Assert.NotNull(review1);
        
        review2 = await context.Reviews.FirstOrDefaultAsync(c => c.Content == "review2");
        Assert.NotNull(review2);
        
        review3 = await context.Reviews.FirstOrDefaultAsync(c => c.Content == "review3");
        Assert.NotNull(review3);
        
        book = await context
            .Books
            .FirstOrDefaultAsync(c => c.Title == "test");
        Assert.NotNull(book);

        var reviews = book.Reviews.ToList();
        Assert.True(reviews.Count > 0);
        
        
        context.Remove(book);
        await context.SaveChangesAsync();
        
        book = await context.Books.FirstOrDefaultAsync(c => c.Title == "test");
        Assert.Null(book);
    }
}