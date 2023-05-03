using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.GenericRepository.Base;
using Products.Application.Domain;
using Products.Application.Domain.Identity;
using Products.Application.Domain.Settings;
using Products.Application.Interfaces.Authentication;
using Utils.Time;

namespace Products.Application.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider, IAuthenticatedUserService authenticatedUserService) :base(options)
        {
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _authenticatedUserService = authenticatedUserService ?? throw new ArgumentNullException(nameof(authenticatedUserService));
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Permission> Permissions => Set<Permission>();

        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<CompanyInfo> CompanyInfo => Set<CompanyInfo>();
        public DbSet<CompanyInfoLogo> CompanyInfoLogo => Set<CompanyInfoLogo>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IApplicationLayer).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property("CreatedAt").CurrentValue = _dateTimeProvider.NowUtc();
                        entry.Property("CreatedBy").CurrentValue = _authenticatedUserService.UserId; 
                        break;
                    case EntityState.Modified:
                        entry.Property("UpdatedAt").CurrentValue = _dateTimeProvider.NowUtc();
                        entry.Property("UpdatedBy").CurrentValue = _authenticatedUserService.UserId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
