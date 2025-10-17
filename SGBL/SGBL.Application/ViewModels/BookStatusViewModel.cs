

using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class BookStatusViewModel : BaseViewModel
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; } = null!;
    }
}
