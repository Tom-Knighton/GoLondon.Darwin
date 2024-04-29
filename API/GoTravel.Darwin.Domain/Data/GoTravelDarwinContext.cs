using GoTravel.Darwin.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace GoTravel.Darwin.Domain.Data;

public class GoTravelDarwinContext: DbContext
{
    public GoTravelDarwinContext() {}
    public GoTravelDarwinContext(DbContextOptions<GoTravelDarwinContext> options): base(options) {}
    
    public virtual DbSet<TimetableEntry> TimetableEntry { get; set; }
    public virtual DbSet<TimetableEntryLocation> TimetableEntryLocation { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TimetableEntry>(e =>
        {
            e.ToTable("TimetableEntries");
            e.HasKey(t => t.RID);

            e.HasIndex(t => t.Operator).IsUnique(false);

            e.HasMany<TimetableEntryLocation>()
                .WithOne(l => l.TimetableEntry)
                .HasForeignKey(l => l.RID);

            e.Property(t => t.StartDate)
                .HasColumnType("date");
        });

        modelBuilder.Entity<TimetableEntryLocation>(e =>
        {
            e.ToTable("TimetableEntryLocations");
            e.Property(l => l.Id).ValueGeneratedOnAdd();
            e.HasKey(l => l.Id);
            e.HasIndex(l => l.Location).IsUnique(false);

            e.Property(l => l.PredictedArrival)
                .HasColumnType("timestamptz");
            e.Property(l => l.PredictedDeparture)
                .HasColumnType("timestamptz");
            e.Property(l => l.ScheduledArrival)
                .HasColumnType("timestamptz");
            e.Property(l => l.ScheduledDeparture)
                .HasColumnType("timestamptz");
        });
    }
}