using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Application.Domain.Settings;

namespace Products.Application.Infrastructure.Persistence.Configuration
{
    public class CompanyInfoConfiguration : IEntityTypeConfiguration<CompanyInfo>
    {
        public void Configure(EntityTypeBuilder<CompanyInfo> builder)
        {
            builder.ToTable(name: "CompanyInfo", schema: "Settings");
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Logo)
                .WithOne(c => c.CompanyInfo)
                .HasForeignKey<CompanyInfoLogo>(c => c.CompanyInfoId)
                .IsRequired();

            builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Country).HasMaxLength(50).IsRequired();
            builder.Property(c => c.State).HasMaxLength(50).IsRequired();
            builder.Property(c => c.City).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Zip).HasMaxLength(10).IsRequired();
            builder.Property(c => c.Line1).HasMaxLength(50).IsRequired();
            builder.Property(c => c.Line2).IsRequired(false).HasMaxLength(50);
        }
    }

    public class CompanyInfoLogoConfiguration : IEntityTypeConfiguration<CompanyInfoLogo>
    {
        public void Configure(EntityTypeBuilder<CompanyInfoLogo> builder)
        {
            builder.ToTable(name: "CompanyInfoLogo",schema: "Settings");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            
            builder.HasOne(l => l.CompanyInfo)
                .WithOne(l => l.Logo)
                .HasForeignKey<CompanyInfoLogo>(l => l.CompanyInfoId)
                .IsRequired();
            
            builder.Property(l => l.Filename).IsRequired(false);
            builder.Property(l => l.FileType).IsRequired(false);
        }
    }

}
