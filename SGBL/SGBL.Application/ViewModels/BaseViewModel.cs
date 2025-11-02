using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public abstract class BaseViewModel<T>
    {
       
        public virtual T Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? Message { get; set; }
    }
}
