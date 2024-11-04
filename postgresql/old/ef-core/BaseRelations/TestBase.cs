using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseRelations
{
    public class TestBase
    {
        static TestBase()
        {
            var sc = new ServiceCollection();
            sc.AddDbContext<AppContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfigurationRoot>();
                var connString = config.GetConnectionString("Default");
                options.UseNpgsql(connString);
            });
            sc.AddSingleton(s => Configure());
            _sp = sc.BuildServiceProvider();
        }

        protected static IServiceProvider _sp;
        
        static IConfigurationRoot Configure()
        {
            var directory = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json");

            var result = builder.Build();
            return result;
        }
    }
}