
namespace SGBL.Application.Dtos
{
    public class BaseAuditableDto<T> : BaseDto<T>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
