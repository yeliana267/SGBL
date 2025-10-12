
namespace SGBL.Domain.Base
{
    public abstract class BaseEntity<Ttype>
    {
        public abstract Type Id { get; set; }
    }
}
