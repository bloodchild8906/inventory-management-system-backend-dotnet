namespace Products.Application.Domain.Settings
{
    public class CompanyInfoLogo
    {
        public int Id { get; set; }
        public string CompanyInfoId { get; set; } = default!;
        public string? Filename { get; set; } = default!;
        public string? FileType { get; set; } = default!;
        public virtual CompanyInfo CompanyInfo { get; set; } = default!;
    }
}
