namespace SGBL.Application.Interfaces
{
    public interface ILoanNotificationService
    {
        Task ProcessDailyNotificationsAsync(CancellationToken cancellationToken = default);
    }
}
