

namespace SGBL.Application.ViewModels
{
    public class NotificationViewModel : BaseViewModel<int>
    {
        public int IdUser { get; set; }
        public int Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public new string Message { get; set; } = string.Empty;
        public int? Status { get; set; }
        public int? IdBook { get; set; }
        public int? IdLoan { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool IsRead => ReadDate.HasValue;
    }
}
