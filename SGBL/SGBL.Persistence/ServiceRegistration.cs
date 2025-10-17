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
    (sp, opt) =>
    {
        if (config.GetValue<bool>("EnableSensitiveDataLogging"))
            opt.EnableSensitiveDataLogging();

        var cs = config.GetConnectionString("SupabaseConnection");

        opt.UseNpgsql(cs, npgsql =>
        {
            // Reintentos ante fallos transitorios de red
            npgsql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            );

            // Más tiempo para consultas (lectura inicial en Supabase puede ser lenta)
            npgsql.CommandTimeout(60);
        });
    },
    contextLifetime: ServiceLifetime.Scoped,
    optionsLifetime: ServiceLifetime.Scoped
);
            #endregion

            #region IoC
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<INationalityRepository, NationalityRepository>();
            services.AddScoped<IUserStatusRepository, UserStatusRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookStatusRepository, BookStatusRepository>();
            services.AddScoped<IReminderStatusRepository, ReminderStatusRepository>();
            services.AddScoped<INotificationStatusRepository, NotificationStatusRepository>();
            services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
            #endregion
        }
    }
}
