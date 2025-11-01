
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Reminders
{
    public class ReminderStatusDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
        
    }

}
