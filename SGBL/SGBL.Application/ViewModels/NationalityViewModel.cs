
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class NationalityViewModel : BaseViewModel<int>
    {
        public override int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;
    }
}
