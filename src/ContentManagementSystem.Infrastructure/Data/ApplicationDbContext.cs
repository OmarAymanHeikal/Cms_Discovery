using Microsoft.EntityFrameworkCore;
using ContentManagementSystem.Core.Entities;

namespace ContentManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Program> Programs { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ProgramCategory> ProgramCategories { get; set; }
    public DbSet<ProgramTag> ProgramTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Program entity
        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(e => e.VideoUrl).HasMaxLength(1000);
            entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Language);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PublishedDate);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Tag entity
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.Comments)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure ProgramCategory many-to-many relationship
        modelBuilder.Entity<ProgramCategory>(entity =>
        {
            entity.HasKey(e => new { e.ProgramId, e.CategoryId });
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.ProgramCategories)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.ProgramCategories)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProgramTag many-to-many relationship
        modelBuilder.Entity<ProgramTag>(entity =>
        {
            entity.HasKey(e => new { e.ProgramId, e.TagId });
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.ProgramTags)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tag)
                  .WithMany(e => e.ProgramTags)
                  .HasForeignKey(e => e.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        var techCategory = new Category { Id = Guid.NewGuid(), Name = "Technology", Description = "Technology related content", Color = "#007bff", IsActive = true };
        var businessCategory = new Category { Id = Guid.NewGuid(), Name = "Business", Description = "Business and entrepreneurship", Color = "#28a745", IsActive = true };
        var cultureCategory = new Category { Id = Guid.NewGuid(), Name = "Culture", Description = "Arts and culture", Color = "#dc3545", IsActive = true };

        modelBuilder.Entity<Category>().HasData(techCategory, businessCategory, cultureCategory);

        // Seed Tags
        var aiTag = new Tag { Id = Guid.NewGuid(), Name = "AI", IsActive = true };
        var innovationTag = new Tag { Id = Guid.NewGuid(), Name = "Innovation", IsActive = true };
        var startupTag = new Tag { Id = Guid.NewGuid(), Name = "Startup", IsActive = true };

        modelBuilder.Entity<Tag>().HasData(aiTag, innovationTag, startupTag);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}