
using Inventory.Application.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Application.Infrastructure.Persistence.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable(name: "Users", schema: "Identity");
        }
    }

    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable(name: "Roles", schema: "Identity");
            builder
                .HasMany(m => m.Permissions)
                .WithOne(m => m.Role);
        }
    }

    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable(name: "RolePermissions", schema: "Identity");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();


            builder
                .HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions);

            builder.HasOne(x => x.Role)
                .WithMany(x => x.Permissions);
        }
    }

    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable(name: "Modules", schema: "Identity");
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Order)
                .IsRequired();

            builder
                .HasMany(x => x.Permissions)
                .WithOne(x => x.Module);
        }
    }

    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(name: "ModulePermissions", schema: "Identity");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.Required).IsRequired();
            builder.HasOne(x => x.Module).WithMany(x => x.Permissions).IsRequired();
        }
    }

    public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable(name: "RoleClaims", schema: "Identity");
        }
    }

    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.ToTable(name: "UserRoles", schema: "Identity");
        }
    }

    public class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable(name: "UserClaims", schema: "Identity");
        }
    }

    public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ToTable(name: "UserLogins", schema: "Identity");
        }
    }

    public class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ToTable(name: "UserTokens", schema: "Identity");
        }
    }
}
