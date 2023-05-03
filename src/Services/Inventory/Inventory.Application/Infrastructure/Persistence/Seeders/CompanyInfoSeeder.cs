using Inventory.Application.Shared.Constants;
using Inventory.Application.Domain.Settings;

namespace Inventory.Application.Infrastructure.Persistence.Seeders
{
    public class CompanyInfoSeeder
    {
        public static async Task CreateDefaultCompanyInfo(ApplicationDbContext context)
        {
            if (context.CompanyInfo.Any())
            {
                return;
            }
            var defaultInfo = new CompanyInfo
            {
                Id = SettingsConstants.CompanyInfoId,
                Name = "NA",
                Phone = "NA",
                Country = "NA",
                State = "NA",
                City = "NA",
                Zip = "NA",
                Line1 = "NA",
                Logo = new CompanyInfoLogo
                {
                    CompanyInfoId = SettingsConstants.CompanyInfoId
                }
            };
            context.CompanyInfo.Add(defaultInfo);
            await context.SaveChangesAsync();
        }
    }
}
