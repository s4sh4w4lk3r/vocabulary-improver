using Microsoft.EntityFrameworkCore;
using ViAPI.Entities;
using static ViAPI.Other.ViConfiguration;

namespace ViAPI.Database;

public partial class ViDbContext : DbContext
{
    public ViDbContext(DbContextOptions<ViDbContext> options) : base(options)
    {
    }

    ILogger? Logger { get; set; } = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ViDbContext));

    public DbSet<User> Users => Set<User>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<ViDictionary> Dictionaries => Set<ViDictionary>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        //optionsBuilder.LogTo(message => Logger.LogInformation(message), new[] { RelationalEventId.CommandExecuted });
    }

    public static void EnsureDatabaseWorking()
    {
        string connstring = GetSecretString(SecretType.MySql);
        var dbOptionsBuilder = new DbContextOptionsBuilder<ViDbContext>().UseMySql(connstring, ServerVersion.AutoDetect(connstring)).Options;
        if (new ViDbContext(dbOptionsBuilder).Database.CanConnect() is false) { throw new InvalidOperationException("Database bad connect."); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_unicode_ci").HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property<uint>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.Property<string>("Discriminator").HasMaxLength(30);
            //Ниже используется подход Table Per Hierarchy, вызывается у базовой сущности User. В бд создается таблица Users с столбцом дискриминатора.
            entity.UseTphMappingStrategy();
        });

        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasBaseType<User>();
            entity.HasIndex(entity => entity.TelegramId, "tgId-unique").IsUnique();
        });

        modelBuilder.Entity<RegistredUser>(entity =>
        {
            entity.HasBaseType<User>();
            entity.HasIndex(e => e.Username, "username-unique").IsUnique();
            entity.HasIndex(e => e.Email, "email-unique").IsUnique();
            entity.Property(e => e.Hash).HasMaxLength(60).IsFixedLength();
            entity.Property(e => e.Firstname).HasMaxLength(255);
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.ToTable("words");
            entity.Property<uint>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.HasIndex(e => e.DictionaryGuid, "dictguid-unique");
            entity.Property(e => e.SourceWord).HasMaxLength(512);
            entity.Property(e => e.TargetWord).HasMaxLength(512);
            entity.Property(e => e.Rating).HasDefaultValue(0);
            entity.ToTable(e => e.HasCheckConstraint("Rating", "Rating > -1 AND Rating < 11").HasName("RatingCheckConstraint"));
            entity.HasOne(e => e.Dictionary).WithMany(e => e.Words).HasForeignKey(e => e.DictionaryGuid).HasPrincipalKey(e => e.Guid).HasConstraintName("WordToDictionary");
        });

        modelBuilder.Entity<ViDictionary>(entity =>
        {
            entity.ToTable("dictionaries");
            entity.Property<uint>("Id");
            entity.HasKey("Id").HasName("PRIMARY");
            entity.HasAlternateKey(e => e.Guid).HasName("GuidKey");
            entity.HasIndex(e => e.UserGuid, "userguid-unique");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany(e => e.Dictionaries).HasForeignKey(e => e.UserGuid).HasPrincipalKey(e => e.Guid).HasConstraintName("DictionaryToUser");
        });
    }
}