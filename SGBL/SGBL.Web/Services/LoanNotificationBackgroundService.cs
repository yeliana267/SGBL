using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGBL.Application.Interfaces;
namespace SGBL.Web.Services
{

    public class LoanNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LoanNotificationBackgroundService> _logger;

        public LoanNotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<LoanNotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ILoanNotificationService>();

                    _logger.LogInformation("Procesando notificaciones de préstamos...");
                    await service.ProcessDailyNotificationsAsync(stoppingToken);
                    _logger.LogInformation("Notificaciones de préstamos procesadas.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando notificaciones de préstamos.");
                }

                // Producción: 1 vez al día
                await Task.Delay(TimeSpan.FromMinutes(24), stoppingToken);

                // Para probar, puedes usar:
                // await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}