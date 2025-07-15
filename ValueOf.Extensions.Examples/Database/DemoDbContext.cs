using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ValueOf.Extensions.EFCore;
using ValueOf.Extensions.Examples.Models;

namespace ValueOf.Extensions.Examples.Database;

public class DemoDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    private readonly ILogger<DemoDbContext> _logger;

    public DemoDbContext(DbContextOptions<DemoDbContext> options, ILogger<DemoDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        ConfigureEntityBase(b);
    }

    private void ConfigureEntityBase(ModelBuilder b)
    {
        foreach (var e in b.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(e.ClrType))
            {
                b.Entity(e.Name).Property(nameof(EntityBase.CreatedAt)).HasColumnType("TIMESTAMP").IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
                b.Entity(e.Name).Property(nameof(EntityBase.ModifiedAt)).HasColumnType("TIMESTAMP").IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
            }
        }
    }

    private IList<User> getSeedUsers()
    {
        return new List<User>
        {
            new User
            {
                Id = UserId.From(12),
                Email = EmailAddress.From("imyuanjian@gmail.com"),
            },
            new User
            {
                Id = UserId.From(22),
                Email = EmailAddress.From("xiaobao@gmail.com"),
            }
        };
    }

    protected override void OnConfiguring(DbContextOptionsBuilder b)
    {
        base.OnConfiguring(b);
        b.UseSnakeCaseNamingConvention();
        b.UseSeeding((ctx, _) =>
        {
            if (!ctx.Set<User>().Any())
            {
                ctx.AddRange(getSeedUsers());
            }

            ctx.SaveChanges();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        base.ConfigureConventions(b);
        b.ConfigureValueOfConversions(typeof(UserId).Assembly);
    }
}

public abstract class EntityBase
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset ModifiedAt { get; set; }
}

public class User : EntityBase
{
    public required UserId Id { get; set; }
    public required EmailAddress Email { get; set; }
}