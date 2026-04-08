using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using QuartzData.Entities;

namespace QuartzData.Context;

public class QuartzDbContext : DbContext
{
    // Add-Migration "Migration Name" -Project QuartzData -StartupProject QuartzAPI
    // Update-Database -Project QuartzData -StartupProject QuartzAPI

    public QuartzDbContext(DbContextOptions<QuartzDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<APICallHistory> ApiCallHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Job entity
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsRequired();
            
            entity.Property(e => e.JobName)
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            entity.Property(e => e.CronExpression)
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.JobData)
                .HasColumnType("nvarchar(max)");
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            
            // Add indexes if needed
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
