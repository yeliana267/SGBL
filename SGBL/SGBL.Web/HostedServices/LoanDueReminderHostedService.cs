using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGBL.Application.Interfaces;
using SGBL.Web.Options;

namespace SGBL.Web.HostedServices
{
    public class LoanDueReminderHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LoanDueReminderHostedService> _logger;
        private readonly IOptionsMonitor<LoanReminderOptions> _optionsMonitor;

        public LoanDueReminderHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<LoanDueReminderHostedService> logger,
            IOptionsMonitor<LoanReminderOptions> optionsMonitor)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _optionsMonitor = optionsMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de recordatorio de préstamos iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var options = _optionsMonitor.CurrentValue;

                if (!options.Enabled)
                {
                    _logger.LogInformation("El recordatorio de préstamos está deshabilitado. Reintentando en 1 día.");
                    await DelayWithCancellation(TimeSpan.FromDays(1), stoppingToken);
                    continue;
                }

                var delay = GetDelayUntilNextRun(options);
                var nextRun = DateTime.Now.Add(delay);
                _logger.LogInformation("Próxima ejecución de recordatorios programada para {NextRun}.", nextRun);

                await DelayWithCancellation(delay, stoppingToken);

                await ProcessRemindersAsync(options, stoppingToken);
            }
        }

        private async Task ProcessRemindersAsync(LoanReminderOptions options, CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var loans = await loanService.GetLoansDueInDays(options.DaysBeforeDueDate);

                if (loans.Count == 0)
                {
                    _logger.LogInformation("No se encontraron préstamos por vencer en {Days} días.", options.DaysBeforeDueDate);
                    return;
                }

                foreach (var loan in loans)
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    var user = await userService.GetById(loan.IdUser);
                    if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    {
                        _logger.LogWarning("El usuario {UserId} del préstamo {LoanId} no tiene correo electrónico configurado.", loan.IdUser, loan.Id);
                        continue;
                    }

                    try
                    {
                        await notificationService.SendLoanDueReminderAsync(loan, user.Email!, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar recordatorio para el préstamo {LoanId}.", loan.Id);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelación solicitada, salir silenciosamente.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar los recordatorios de préstamos.");
            }
        }

        private static TimeSpan GetDelayUntilNextRun(LoanReminderOptions options)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.Add(options.GetDailyRunTime());

            if (nextRun <= now)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun - now;
        }

        private static async Task DelayWithCancellation(TimeSpan delay, CancellationToken token)
        {
            if (delay <= TimeSpan.Zero)
            {
                return;
            }

            await Task.Delay(delay, token);
        }
    }
}
