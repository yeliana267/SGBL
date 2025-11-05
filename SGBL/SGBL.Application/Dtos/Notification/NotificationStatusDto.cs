

namespace SGBL.Application.Dtos.Notification
{
    public class NotificationStatusDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
        
    }

}
