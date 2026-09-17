using System.Collections.ObjectModel;
using testWPF.Data;
using testWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace testWPF.Services;

/// <summary>
/// SQLite-backed registry service for tuberculosis case management.
/// </summary>
public class RegistryService
{
    public RegistryService()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var db = new AppDbContext();
        try
        {
            db.Database.Migrate();
        }
        catch (Exception)
        {
            db.Database.EnsureCreated();
        }

        // Ensure the AnnotationsJson column exists (handles cases where
        // migrations didn't run or the DB was created before this column was added)
        try
        {
            db.Database.ExecuteSqlRaw(
                "ALTER TABLE Cases ADD COLUMN AnnotationsJson TEXT");
        }
        catch (Exception)
        {
            // Column already exists — safe to ignore
        }
    }

    public ObservableCollection<TuberculosisCase> GetAll()
    {
        using var db = new AppDbContext();
        var list = db.Cases.OrderBy(c => c.NumeroOrdre).ToList();
        return new ObservableCollection<TuberculosisCase>(list);
    }

    public void Add(TuberculosisCase tbCase)
    {
        using var db = new AppDbContext();
        tbCase.Id = 0; // auto-increment
        db.Cases.Add(tbCase);
        db.SaveChanges();
    }

    public void Update(TuberculosisCase tbCase)
    {
        using var db = new AppDbContext();
        var existing = db.Cases.FirstOrDefault(c => c.Id == tbCase.Id);
        if (existing != null)
        {
            existing.CopyFrom(tbCase);
            db.SaveChanges();
        }
    }

    public void Delete(int caseId)
    {
        using var db = new AppDbContext();
        var tbCase = db.Cases.FirstOrDefault(c => c.Id == caseId);
        if (tbCase != null)
        {
            db.Cases.Remove(tbCase);
            db.SaveChanges();
        }
    }

    public int GetNextNumeroOrdre()
    {
        using var db = new AppDbContext();
        if (!db.Cases.Any()) return 1;
        return db.Cases.Max(c => c.NumeroOrdre ?? 0) + 1;
    }
}
