using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using ViAPI.Entities;
using ViAPI.StaticMethods;

namespace ViAPI.Database;

public class ViDbContext : DbContext
{
    public ViDbContext()
    {
        if (Database.CanConnect() is false) throw new Exception("Bad connection attempt.");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<ViDictionary> Dictionaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Здесь сначала читаются параметры из appsettings.json, а там уже путь на секреты, в которых и есть строка подключения.
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        string secretsPath = builder.GetSection("ViApiSettings")["SecretsPath"]!;
        InputChecker.CheckStringException(secretsPath);

        builder = new ConfigurationBuilder().AddJsonFile(secretsPath).Build();
        string connstring = builder.GetConnectionString("MySql")!;
        InputChecker.CheckStringException(connstring);

        optionsBuilder.UseMySql(connstring, ServerVersion.AutoDetect(connstring));

        optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });

        optionsBuilder.UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_unicode_ci").HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Guid).HasName("PRIMARY");
            //Ниже используется подход Table Per Hierarchy, вызывается у базовой сущности User. В бд создается таблица Users с столбцом дискриминатора.
            entity.UseTphMappingStrategy();
        });

        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasIndex(entity => entity.TelegramId, "tgId-unique").IsUnique();
        });

        modelBuilder.Entity<RegistredUser>(entity =>
        {
            entity.HasIndex(e => e.Username, "username-unique").IsUnique();
            entity.HasIndex(e => e.Email, "email-unique").IsUnique();
            entity.Property(e => e.Hash).HasMaxLength(40).IsFixedLength();
            entity.Property(e => e.Firstname).HasMaxLength(255);
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.ToTable("words");
            entity.HasKey(e => e.Guid).HasName("PRIMARY");
            entity.HasIndex(e => e.DictionaryGuid, "dictguid-unique");
            entity.Property(e => e.SourceWord).HasMaxLength(512);
            entity.Property(e => e.TargetWord).HasMaxLength(512);
            entity.Property(e => e.Rating).HasDefaultValue(0);
            entity.ToTable(e => e.HasCheckConstraint("Rating", "Rating > -1 AND Rating < 11").HasName("RatingCheckConstraint"));
            entity.HasOne(e => e.Dictionary).WithMany(e => e.Words).HasForeignKey(e => e.DictionaryGuid).HasConstraintName("WordToDictionary");
        });

        modelBuilder.Entity<ViDictionary>(entity =>
        {
            entity.ToTable("dictionaries");
            entity.HasKey(e => e.Guid).HasName("PRIMARY");
            entity.HasIndex(e => e.UserGuid, "userguid-unique");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany(e => e.Dictionaries).HasForeignKey(e => e.UserGuid).HasConstraintName("DictionaryToUser");
        });
    }
}