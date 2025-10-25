

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Infraestructure;

namespace SGBL.Application
{
    public static class ServiceRegistration
    {
        public static void AddInfraestructureLayerIoc(this IServiceCollection services)
        {
             services.AddSingleton<IServiceLogs, ServiceLogs>();
        }

    }
}

