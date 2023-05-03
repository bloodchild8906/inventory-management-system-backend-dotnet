namespace Persistence.GenericRepository.Base
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAt { get; protected set; }
        public string? CreatedBy { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public string? UpdatedBy { get; protected set; }
    }
}
