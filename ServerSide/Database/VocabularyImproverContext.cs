using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ServerSide.Database;

public partial class VocabularyImproverContext : DbContext
{
    public VocabularyImproverContext()
    {
    }

    public VocabularyImproverContext(DbContextOptions<VocabularyImproverContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Word> Words { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=admin;password=admin;database=vocabulary-improver", ServerVersion.Parse("8.0.30-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserGuid).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserGuid).HasColumnName("user_guid");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .HasColumnName("first_name");
            entity.Property(e => e.RedDate).HasColumnName("red_date");
            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PRIMARY");

            entity.ToTable("words");

            entity.HasIndex(e => e.UserGuidFk, "word_to_user");

            entity.Property(e => e.WordId).HasColumnName("word_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserGuidFk).HasColumnName("user_guid_fk");
            entity.Property(e => e.Word1)
                .HasMaxLength(256)
                .HasColumnName("word1");
            entity.Property(e => e.Word2)
                .HasMaxLength(256)
                .HasColumnName("word2");

            entity.HasOne(d => d.UserGuidFkNavigation).WithMany(p => p.Words)
                .HasForeignKey(d => d.UserGuidFk)
                .HasConstraintName("word_to_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
