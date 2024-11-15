using Microsoft.EntityFrameworkCore;
using Repro.Worker.Model;

namespace Repro.Worker;

public class PostgresContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Model.Lead> Leads => Set<Model.Lead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.ToTable("leads");
            entity.HasKey(e => e.Id);
            entity.Property(p => p.HistoryJson).HasColumnType("json");
            entity.Property(p => p.HistoryJsonB).HasColumnType("jsonb");
        });
    }

}
