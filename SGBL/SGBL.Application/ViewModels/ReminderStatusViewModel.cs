

using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class ReminderStatusViewModel : BaseViewModel<int>
    {

        [StringLength(50)]
        [Required]
        public string Name { get; set; } = null!;
    }
}
