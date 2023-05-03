namespace Persistence.GenericRepository.Base
{
    public class BaseEntity<TId> : AuditableEntity
    {
        public TId Id { get; protected set; } = default!;
    }
}
