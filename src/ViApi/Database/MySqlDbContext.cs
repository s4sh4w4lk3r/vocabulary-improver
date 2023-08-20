using Microsoft.EntityFrameworkCore;

namespace ViApi.Database;

public class MySqlDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Tom", Age = 37 },
                new User { Id = 2, Name = "Bob", Age = 41 },
                new User { Id = 3, Name = "Sam", Age = 24 }
        );
    }
}
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = ""; // имя пользователя
    public int Age { get; set; } // возраст пользователя
}