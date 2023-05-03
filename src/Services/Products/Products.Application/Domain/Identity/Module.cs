namespace Products.Application.Domain.Identity
{
    public class Module
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Order { get; set; } = default!;
        public virtual ICollection<Permission> Permissions { get; set; } = default!;

    }
}
