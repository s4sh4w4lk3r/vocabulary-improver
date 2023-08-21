using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Users;

namespace ViApi.Database.MySql;

public class MySqlDbContext : DbContext
{
    public DbSet<UserBase> Users => Set<UserBase>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<Dictionary> Dictionaries => Set<Dictionary>();

    public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_unicode_ci").HasCharSet("utf8mb4");

        modelBuilder.Entity<UserBase>(entity =>
        {
            entity.ToTable("users");
            entity.Property<int>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.Property<string>("Discriminator").HasMaxLength(30);
            //Ниже используется подход Table Per Hierarchy, вызывается у базовой сущности User. В бд создается таблица Users с столбцом дискриминатора.
            entity.UseTphMappingStrategy();
        });

        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasBaseType<UserBase>();
            entity.HasIndex(entity => entity.TelegramId, "tgId-unique").IsUnique();
        });

        modelBuilder.Entity<ApiUser>(entity =>
        {
            entity.HasBaseType<UserBase>();
            entity.HasIndex(e => e.Username, "username-unique").IsUnique();
            entity.HasIndex(e => e.Email, "email-unique").IsUnique();
            entity.Property(e => e.Password).HasMaxLength(60).IsFixedLength();
            entity.Property(e => e.Firstname).HasMaxLength(255);
        });

#error надо в типы добавить нав. свойства и допилить этот метод.
    }
}