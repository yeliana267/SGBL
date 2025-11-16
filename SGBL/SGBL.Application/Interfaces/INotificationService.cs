
using System.Threading;
using System.Threading.Tasks;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Notification;

namespace SGBL.Application.Interfaces
{
    public interface INotificationService : IGenericService<NotificationDto>
    {
        Task SendLoanDueReminderAsync(LoanDto loan, string email, CancellationToken cancellationToken = default);
    }
}
