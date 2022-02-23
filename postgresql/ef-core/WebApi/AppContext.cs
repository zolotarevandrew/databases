using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApi;

public class ApiContext : DbContext
{
    public DbSet<UserAggregate> Users { get; set; }

    public ApiContext(DbContextOptions<ApiContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAggregate>(entity =>
        {
            entity.ToTable("users_test");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                /*.HasConversion(new ValueConverter<UserId, int>(
                    v => v.Value,
                    v => new UserId(v)))
                
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore)*/;

            entity.OwnsOne(c => c.Address, e =>
            {
                e.Property(c => c.City).HasColumnName("city");
                e.Property(c => c.Street).HasColumnName("street");
                e.Property(c => c.Home).HasColumnName("home");
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}