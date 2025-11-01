

using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class NotificationTypeViewModel : BaseViewModel<int>
    {
        public override int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; } = null!;
    }
}
