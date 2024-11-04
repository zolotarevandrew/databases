using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>((sp, options) =>
{
    var connString = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(connString);
});

var app = builder.Build();
app.MapGet("/", async (ApiContext context) =>
{
    context.Users.Add(new UserAggregate(new Address("city", "street", "home")));
    await context.SaveChangesAsync();
    return "ok";
});

app.MapGet("/get", async (ApiContext context) =>
{
    var users = await context.Users.AsNoTracking().CountAsync(c => c.Address.City == "city");
    return users;
});

app.MapPut("/address/{id}", async (ApiContext context, int id) =>
{
    try
    {
        var found = await context.Users.FirstOrDefaultAsync(c => c.Id == id);
        var newAddress = new Address("city1", "street1", "home");
        found.ChangeAddress(newAddress);

        context.Users.Update(found);
        await context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        return "conflict";
    }
    
    return "ok";
});

app.Run();