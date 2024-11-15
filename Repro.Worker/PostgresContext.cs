using Microsoft.EntityFrameworkCore;
using Repro.Worker.Model;

namespace Repro.Worker;

public class PostgresContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Model.Lead> Leads => Set<Model.Lead>();

    public DbSet<Model.History> History => Set<Model.History>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("leads");
            entity.HasKey(e => e.Id);
            entity.Property(p => p.LastHistory).HasColumnType("jsonb"); // Or JSON

            entity.HasMany(e => e.History).WithOne();
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.ToTable("leads_history");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Stage).HasConversion<string>();
            entity.HasDiscriminator(e => e.Stage)
                .HasValue<Model.HistoryMatched>(HistoryStage.Matched)
                .HasValue<Model.HistoryComplete>(HistoryStage.Complete)
                .IsComplete(true);
        });
    }

}
