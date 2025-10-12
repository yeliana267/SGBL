using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;
using SGBL.Persistence.Repositories;

namespace SGBL.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Db
            var connectionString = config.GetConnectionString("SupabaseConnection");

            services.AddDbContext<SGBLContext>(
                (serviceProvider, opt) =>
                {
                    if (config.GetValue<bool>("EnableSensitiveDataLogging"))
                        opt.EnableSensitiveDataLogging();

                    opt.UseNpgsql(connectionString, m =>
                        m.MigrationsAssembly(typeof(SGBLContext).Assembly.FullName));
                },
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Scoped
            );
            #endregion

            #region IoC
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBookRepository, BookRepository>();
            #endregion
        }
    }
}
