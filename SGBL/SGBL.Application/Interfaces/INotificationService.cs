
using System.Threading;
using System.Threading.Tasks;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Notification;
using System.Threading;
using System.Threading.Tasks;
using SGBL.Application.Dtos.Loan;

namespace SGBL.Application.Interfaces
{
    public interface INotificationService : IGenericService<NotificationDto>
    {
        Task SendLoanDueReminderAsync(LoanDto loan, string email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<NotificationDto>> GetRecentByUserAsync(int userId, int take = 5, bool onlyUnread = true);
    }
}
