
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class NationalityViewModel : BaseViewModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}
