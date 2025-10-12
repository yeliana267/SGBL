
namespace SGBL.Domain.Base
{
    public abstract class BaseEntity<Type>: AuditEntity
    {
        public abstract Type Id { get; set; }
    }
}
