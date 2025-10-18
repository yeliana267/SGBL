
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Reminders
{
    public class ReminderStatusDto : BaseDto
    {

            [Required, StringLength(20)]
            public string Name { get; set; } = default!;
        
    }

}
