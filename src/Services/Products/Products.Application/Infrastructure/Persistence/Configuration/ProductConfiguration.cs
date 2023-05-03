using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Application.Domain;
using Products.Application.Domain.Identity;

namespace Products.Application.Infrastructure.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Property(p => p.CreatedAt);
            builder.Property(p => p.CreatedBy);
            
            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.CreatedBy);

            builder.Property(p => p.UpdatedAt);
            builder.Property(p => p.UpdatedBy);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UpdatedBy);
        }
    }
}
