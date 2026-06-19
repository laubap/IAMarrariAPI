using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TagIaConfig> TagsIaConfig { get; set; } = null!;

    public DbSet<TagPerfilIa> PerfisIa { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TagIaConfig>()
            .HasIndex(x => new { x.ClienteId, x.TagName })
            .IsUnique();

        // Impede perfis duplicados para a mesma tag do mesmo cliente
        modelBuilder.Entity<TagPerfilIa>()
            .HasIndex(x => new { x.ClienteId, x.TagName })
            .IsUnique();
    }
}