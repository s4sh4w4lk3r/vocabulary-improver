using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ViAPI.ScaffoldModels;

public partial class VocabularyImproverContext : DbContext
{
    public VocabularyImproverContext()
    {
    }

    public VocabularyImproverContext(DbContextOptions<VocabularyImproverContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dictionary> Dictionaries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserdataReg> UserdataRegs { get; set; }

    public virtual DbSet<UserdataTg> UserdataTgs { get; set; }

    public virtual DbSet<Word> Words { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Dictionary>(entity =>
        {
            entity.HasKey(e => e.DictionaryGuidPk).HasName("PRIMARY");

            entity.ToTable("dictionaries");

            entity.HasIndex(e => e.UserGuidFk, "dict_to_user");

            entity.Property(e => e.DictionaryGuidPk).HasColumnName("dictionary_guid_pk");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UserGuidFk).HasColumnName("user_guid_fk");

            entity.HasOne(d => d.UserGuidFkNavigation).WithMany(p => p.Dictionaries)
                .HasForeignKey(d => d.UserGuidFk)
                .HasConstraintName("dict_to_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserGuidPk).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserGuidPk).HasColumnName("user_guid_pk");
            entity.Property(e => e.IsTelegram).HasColumnName("is_telegram");
        });

        modelBuilder.Entity<UserdataReg>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("userdata_reg");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.UserGuidFk, "userdata_reg_to_users");

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .HasColumnName("firstname");
            entity.Property(e => e.Hash)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("hash");
            entity.Property(e => e.UserGuidFk).HasColumnName("user_guid_fk");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.UserGuidFkNavigation).WithMany()
                .HasForeignKey(d => d.UserGuidFk)
                .HasConstraintName("userdata_reg_to_users");
        });

        modelBuilder.Entity<UserdataTg>(entity =>
        {
            entity.HasKey(e => e.TelegramId).HasName("PRIMARY");

            entity.ToTable("userdata_tg");

            entity.HasIndex(e => e.UserGuidFk, "user_guid_fk").IsUnique();

            entity.Property(e => e.TelegramId)
                .ValueGeneratedNever()
                .HasColumnName("telegram_id");
            entity.Property(e => e.UserGuidFk).HasColumnName("user_guid_fk");

            entity.HasOne(d => d.UserGuidFkNavigation).WithOne(p => p.UserdataTg)
                .HasForeignKey<UserdataTg>(d => d.UserGuidFk)
                .HasConstraintName("user_guid_fk_tg_to_user_guid_pk");
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.WordGuidPk).HasName("PRIMARY");

            entity.ToTable("words");

            entity.HasIndex(e => e.DictionaryGuidFk, "word_to_dict");

            entity.Property(e => e.WordGuidPk).HasColumnName("word_guid_pk");
            entity.Property(e => e.DictionaryGuidFk).HasColumnName("dictionary_guid_fk");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.SourceWord)
                .HasMaxLength(512)
                .HasColumnName("source_word");
            entity.Property(e => e.TargetWord)
                .HasMaxLength(512)
                .HasColumnName("target_word");

            entity.HasOne(d => d.DictionaryGuidFkNavigation).WithMany(p => p.Words)
                .HasForeignKey(d => d.DictionaryGuidFk)
                .HasConstraintName("word_to_dict");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
