using Microsoft.EntityFrameworkCore;
using testWPF.Models;

namespace testWPF.Data;

/// <summary>
/// Entity Framework Core DbContext for the Tuberculosis Registry SQLite database.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<TuberculosisCase> Cases => Set<TuberculosisCase>();

    public string DbPath { get; }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "tb_registry.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TuberculosisCase>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // Lung annotation fields
            entity.Property(e => e.LungDrawing).HasColumnType("BLOB");
            entity.Property(e => e.AnnotationsJson).HasColumnType("TEXT");
        });
    }
}
