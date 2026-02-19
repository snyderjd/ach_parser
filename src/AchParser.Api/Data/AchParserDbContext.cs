
using AchParser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AchParser.Api.Data;

public class AchParserDbContext : DbContext
{
    public AchParserDbContext(DbContextOptions<AchParserDbContext> options) : base(options) { }

    public DbSet<AchFile> AchFiles { get; set; } = null!;
    public DbSet<FileHeader> FileHeaders { get; set; } = null!;
    public DbSet<FileControl> FileControls { get; set; } = null!;
    public DbSet<BatchHeader> BatchHeaders { get; set; } = null!;
    public DbSet<BatchControl> BatchControls { get; set; } = null!;
    public DbSet<EntryDetail> EntryDetails { get; set; } = null!;
    public DbSet<Addenda> Addendas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Snake_case naming convention should be configured in Program.cs via Npgsql options, not here.

        // AchFile
        modelBuilder.Entity<AchFile>(entity =>
        {
            entity.ToTable("ach_files");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Filename).IsRequired();
            entity.Property(e => e.Hash).IsRequired();
            entity.Property(e => e.UnparsedFile).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.FileHeader)
                .WithOne(e => e.AchFile)
                .HasForeignKey<FileHeader>(e => e.AchFileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.FileControl)
                .WithOne(e => e.AchFile)
                .HasForeignKey<FileControl>(e => e.AchFileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.BatchHeaders)
                .WithOne(e => e.AchFile)
                .HasForeignKey(e => e.AchFileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Hash).IsUnique();
        });

        // FileHeader
        modelBuilder.Entity<FileHeader>(entity =>
        {
            entity.ToTable("file_headers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImmediateDestination).IsRequired();
            entity.Property(e => e.ImmediateOrigin).IsRequired();
            entity.Property(e => e.FileCreationDate).HasColumnType("date").IsRequired();
            entity.Property(e => e.FileCreationTime).HasColumnType("time").IsRequired();
            entity.Property(e => e.ImmediateDestinationName).IsRequired();
            entity.Property(e => e.ImmediateOriginName).IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
            entity.HasIndex(e => e.AchFileId).IsUnique();
        });

        // FileControl
        modelBuilder.Entity<FileControl>(entity =>
        {
            entity.ToTable("file_controls");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BatchCount).IsRequired();
            entity.Property(e => e.BlockCount).IsRequired();
            entity.Property(e => e.EntryAddendaCount).IsRequired();
            entity.Property(e => e.TotalDebit).HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.TotalCredit).HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
            entity.HasIndex(e => e.AchFileId).IsUnique();
        });

        // BatchHeader
        modelBuilder.Entity<BatchHeader>(entity =>
        {
            entity.ToTable("batch_headers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceClassCode).HasColumnType("char(3)").IsRequired();
            entity.Property(e => e.CompanyName).IsRequired();
            entity.Property(e => e.CompanyIdentification).IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
            entity.HasMany(e => e.EntryDetails)
                .WithOne(e => e.BatchHeader)
                .HasForeignKey(e => e.BatchHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.BatchControl)
                .WithOne(e => e.BatchHeader)
                .HasForeignKey<BatchControl>(e => e.BatchHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BatchControl
        modelBuilder.Entity<BatchControl>(entity =>
        {
            entity.ToTable("batch_controls");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntryAddendaCount).IsRequired();
            entity.Property(e => e.TotalDebit).HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.TotalCredit).HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
            entity.HasIndex(e => e.BatchHeaderId).IsUnique();
        });

        // EntryDetail
        modelBuilder.Entity<EntryDetail>(entity =>
        {
            entity.ToTable("entry_details");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoutingNumber).HasColumnType("char(9)").IsRequired();
            entity.Property(e => e.AccountNumber).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.IndividualName).IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
            entity.HasMany(e => e.Addendas)
                .WithOne(e => e.EntryDetail)
                .HasForeignKey(e => e.EntryDetailId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Addenda
        modelBuilder.Entity<Addenda>(entity =>
        {
            entity.ToTable("addendas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Information).IsRequired();
            entity.Property(e => e.LineNumber).IsRequired();
            entity.Property(e => e.UnparsedRecord).HasColumnType("char(94)").IsRequired();
        });
    }
}