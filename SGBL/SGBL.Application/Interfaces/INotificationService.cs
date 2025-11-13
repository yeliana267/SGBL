
using SGBL.Application.Dtos.Notification;
using System.Threading;
using System.Threading.Tasks;
using SGBL.Application.Dtos.Loan;

namespace SGBL.Application.Interfaces
{
    public interface INotificationService : IGenericService<NotificationDto>
    {
        Task<NotificationDto?> CreateLoanNotificationAsync(int userId, int loanId, int typeId, string title, string message, bool sendEmail = false, string? email = null);
        Task<NotificationDto?> CreateBookNotificationAsync(int userId, int bookId, int typeId, string title, string message, bool sendEmail = false, string? email = null);
        Task<NotificationDto?> CreateUserNotificationAsync(int userId, int typeId, string title, string message, bool sendEmail = false, string? email = null);
        Task<List<NotificationDto>> GetByUserAsync(int userId);
        Task<List<NotificationDto>> GetUnreadByUserAsync(int userId);
        Task<int> CountUnreadByUserAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
        Task SendLoanDueReminderAsync(LoanDto loan, string email, CancellationToken cancellationToken = default);

    }
}
