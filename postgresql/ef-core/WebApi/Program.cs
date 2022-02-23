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

app.Run();