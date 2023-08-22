using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Users;

namespace ViApi.Services.MySql;

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

        modelBuilder.Entity<Word>(entity =>
        {
            entity.ToTable("words");
            entity.Property<int>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.HasIndex(e => e.DictionaryGuid, "dictguid-unique");
            entity.Property(e => e.SourceWord).HasMaxLength(512);
            entity.Property(e => e.TargetWord).HasMaxLength(512);
            entity.Property(e => e.Rating).HasDefaultValue(0);
            entity.ToTable(e => e.HasCheckConstraint("Rating", "Rating > -1 AND Rating < 11").HasName("RatingCheckConstraint"));
            entity.HasOne(e => e.Dictionary).WithMany(e => e.Words).HasForeignKey(e => e.DictionaryGuid).HasPrincipalKey(e => e.Guid).HasConstraintName("WordToDictionary");
        });

        modelBuilder.Entity<Dictionary>(entity =>
        {
            entity.ToTable("dictionaries");
            entity.Property<int>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.HasIndex(e => e.UserGuid, "userguid-unique");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany(e => e.Dictionaries).HasForeignKey(e => e.UserGuid).HasPrincipalKey(e => e.Guid).HasConstraintName("DictionaryToUser");
        });
    }
}