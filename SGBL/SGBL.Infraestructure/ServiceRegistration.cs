

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGBL.Application.Interfaces;
using SGBL.Domain.Settings;
using SGBL.Infraestructure;
using SGBL.Infraestructure.Services;


namespace SGBL.Application
{
    public static class ServiceRegistration
    {
        public static void AddInfraestructureLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));

            services.AddSingleton<IServiceLogs, ServiceLogs>();
            services.AddScoped<IEmailService, EmailService>();
        }

    }
}

