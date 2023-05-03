namespace Products.Application.Domain.Settings
{
    public class CompanyInfo
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Zip { get; set; } = default!;
        public string Line1 { get; set; } = default!;
        public string Line2 { get; set; } = default!;
        public virtual CompanyInfoLogo Logo { get; set; } = default!;
    }
}
