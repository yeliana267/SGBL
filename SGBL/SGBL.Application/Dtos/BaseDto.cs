
namespace SGBL.Application.Dtos
{
    public abstract class BaseDto <T>
    {
        public virtual T Id { get; set; } = default!;

    }
}
