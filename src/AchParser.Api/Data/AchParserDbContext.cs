
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
        // Some of these are supposed to be one-to-one relationships but I kept running into issues
        // creating migrations with .UseSnakeCaseNamingConvention getting the following error:
        
        // Unable to create a 'DbContext' of type ''. The exception 'The object has been 
        // removed from the model.' was thrown while attempting to create an instance. For 
        // the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728



        // Explicitly set plural table names
        modelBuilder.Entity<AchFile>().ToTable("ach_files");
        modelBuilder.Entity<FileHeader>().ToTable("file_headers");
        modelBuilder.Entity<FileControl>().ToTable("file_controls");
        modelBuilder.Entity<BatchHeader>().ToTable("batch_headers");
        modelBuilder.Entity<BatchControl>().ToTable("batch_controls");
        modelBuilder.Entity<EntryDetail>().ToTable("entry_details");
        modelBuilder.Entity<Addenda>().ToTable("addendas");

        // AchFile <-> FileHeader (one-to-many)
        modelBuilder.Entity<FileHeader>()
            .HasOne(fh => fh.AchFile)
            .WithMany(af => af.FileHeaders)
            .HasForeignKey(fh => fh.AchFileId);

        // AchFile <-> FileControl (one-to-many)
        modelBuilder.Entity<FileControl>()
            .HasOne(fc => fc.AchFile)
            .WithMany(af => af.FileControls)
            .HasForeignKey(fc => fc.AchFileId);

        // AchFile <-> BatchHeader (one-to-many)
        modelBuilder.Entity<BatchHeader>()
            .HasOne(bh => bh.AchFile)
            .WithMany(af => af.BatchHeaders)
            .HasForeignKey(bh => bh.AchFileId);

        // BatchHeader <-> BatchControl (one-to-many)
        modelBuilder.Entity<BatchControl>()
            .HasOne(bc => bc.BatchHeader)
            .WithMany(bh => bh.BatchControls)
            .HasForeignKey(bc => bc.BatchHeaderId);

        // BatchHeader <-> EntryDetail (one-to-many)
        modelBuilder.Entity<EntryDetail>()
            .HasOne(ed => ed.BatchHeader)
            .WithMany(bh => bh.EntryDetails)
            .HasForeignKey(ed => ed.BatchHeaderId);

        // EntryDetail <-> Addenda (one-to-many)
        modelBuilder.Entity<Addenda>()
            .HasOne(a => a.EntryDetail)
            .WithMany(ed => ed.Addendas)
            .HasForeignKey(a => a.EntryDetailId);

    }
}