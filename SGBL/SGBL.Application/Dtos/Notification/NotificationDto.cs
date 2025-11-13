
namespace SGBL.Application.Dtos.Notification
{
    public class NotificationDto : BaseAuditableDto<int>
    {
        public int IdUser { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int IdBook { get; set; }
        public int IdLoan { get; set; }
        public DateTime ReadDate { get; set; }
    }
}
