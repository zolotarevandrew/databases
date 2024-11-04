using System;
using BaseRelations.ManyToMany;
using BaseRelations.OneToMany;
using BaseRelations.OneToOne;
using Microsoft.EntityFrameworkCore;

namespace BaseRelations;

public class AppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    public DbSet<UserBooks> UserBooks { get; set; }
    
    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.LogTo(Console.WriteLine);
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.UserName)
                .HasColumnName("username")
                .HasColumnType("text");
            
            entity
                .HasOne(c => c.Address)
                .WithOne(c => c.User)
                .HasForeignKey<Address>( c => c.UserId);
        });
        
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.Street).HasColumnName("street");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Title)
                .HasColumnName("title");

            entity
                .HasMany(c => c.Reviews)
                .WithOne(e => e.Book);
        });
        
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            
            entity.Property(e => e.Content)
                .HasColumnName("content");

            entity
                .HasOne(c => c.Book)
                .WithMany(e => e.Reviews)
                .HasForeignKey( c => c.BookId);
        });
        
        modelBuilder.Entity<UserBooks>(entity =>
        {
            entity.ToTable("users_books");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            
            entity.Property(e => e.BookId)
                .HasColumnName("book_id");
            
            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity
                .HasOne(c => c.Book)
                .WithMany(c => c.UserBooks)
                .HasForeignKey(c => c.BookId);
            
            entity
                .HasOne(c => c.User)
                .WithMany(c => c.UserBooks)
                .HasForeignKey(c => c.UserId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}