
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Notification
{
    public class NotificationTypeDto : BaseDto
    {

            [Required, StringLength(20)]
            public string Name { get; set; } = default!;
        
    }

}
