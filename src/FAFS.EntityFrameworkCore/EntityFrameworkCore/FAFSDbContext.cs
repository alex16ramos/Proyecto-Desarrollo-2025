using FAFS.Destinations;
using FAFS.Experiences;
using FAFS.Favorites;
using FAFS.Metrics;
using FAFS.Notifications;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users;
using FAFS;

namespace FAFS.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class FAFSDbContext : AbpDbContext<FAFSDbContext>, IIdentityDbContext
{
    private const string Schema = "Abp";
    private readonly ICurrentUser? _currentUser;

    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Destination> Destinations { get; set; }
    public DbSet<DestinationRating> DestinationRatings { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<FavoriteDestination> FavoriteDestinations { get; set; }
    public DbSet<ApiUsageMetric> ApiUsageMetrics { get; set; }
    public DbSet<AppNotification> AppNotifications { get; set; }

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public FAFSDbContext(DbContextOptions<FAFSDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<Destination>(b =>
        {
            b.ToTable("Destination", Schema);
            b.ConfigureByConvention();

            b.Property(d => d.Name).IsRequired().HasMaxLength(200);
            b.Property(d => d.Country).IsRequired().HasMaxLength(100);
            b.Property(d => d.City).IsRequired().HasMaxLength(100);
            b.Property(d => d.Description).HasMaxLength(2000);
            b.Property(d => d.PhotoUrl).HasMaxLength(500);
            b.Property(d => d.LastUpdated);

            b.OwnsOne(d => d.Coordinates, c =>
            {
                c.Property(p => p.Latitude)
                 .HasColumnName("Latitude")
                 .IsRequired()
                 .HasMaxLength(50);

                c.Property(p => p.Longitude)
                 .HasColumnName("Longitude")
                 .IsRequired()
                 .HasMaxLength(50);
            });
        });

        builder.Entity<DestinationRating>(b =>
        {
            b.ToTable("DestinationRatings", Schema);
            b.ConfigureByConvention();

            b.Property(x => x.Score).IsRequired();
            b.Property(x => x.Comment).HasMaxLength(1000);

            b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique(false);
            b.HasIndex(x => x.DestinationId);
        });

        builder.Entity<Experience>(b =>
        {
            b.ToTable("Experiences", Schema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).IsRequired().HasMaxLength(4000);
            b.Property(x => x.Comments).HasMaxLength(2000);
            b.Property(x => x.Rating).IsRequired();

            b.HasIndex(x => x.DestinationId);
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<FavoriteDestination>(b =>
        {
            b.ToTable("FavoriteDestinations", Schema);
            b.ConfigureByConvention();

            b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique(true);
        });

        builder.Entity<ApiUsageMetric>(b =>
        {
            b.ToTable("ApiUsageMetrics", Schema);
            b.ConfigureByConvention();

            b.Property(x => x.Endpoint).IsRequired().HasMaxLength(300);
            b.Property(x => x.HttpMethod).IsRequired().HasMaxLength(10);
            b.Property(x => x.IpAddress).HasMaxLength(50);

            b.HasIndex(x => x.Timestamp);
            b.HasIndex(x => x.Endpoint);
        });

        builder.Entity<AppNotification>(b =>
        {
            b.ToTable("AppNotifications", Schema);
            b.ConfigureByConvention();

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Message).IsRequired().HasMaxLength(2000);
            b.Property(x => x.Type).HasMaxLength(50);
            b.Property(x => x.IsRead).IsRequired();

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.IsRead });
        });
    }
}
