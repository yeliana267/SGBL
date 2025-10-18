

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;

namespace SGBL.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<INationalityService, NationalityService>();
            services.AddScoped<IUserStatusService, UserStatusService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookStatusService, BookStatusService>();
            services.AddScoped<IReminderStatusService, ReminderStatusService>();
            services.AddScoped<INotificationStatusService, NotificationStatusService>();
            services.AddScoped<INotificationTypeService, NotificationTypeService>();
        }

    }
}

