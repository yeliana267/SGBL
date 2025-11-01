
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Notification
{
    public class NotificationTypeDto : BaseAuditableDto<int>
    {

        public string Name { get; set; } = default!;
        
    }

}
