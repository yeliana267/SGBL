using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class BaseViewModel
    {
       
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? Message { get; set; }
    }
}
