
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class UserStatusViewModel : BaseViewModel<int>
    {
        public override int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = default!;

    }
}
