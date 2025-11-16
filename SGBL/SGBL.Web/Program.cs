using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGBL.Application;
using SGBL.Persistence;
using SGBL.Persistence.Context;
using SGBL.Web.HostedServices;
using SGBL.Web.Options;
using System.Security.Claims;

namespace SGBL.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration;
            var connectionString = config.GetConnectionString("SupabaseConnection");

            //  SERVICIOS MVC Y API
            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();

            //  CONFIGURACIÓN DE AUTENTICACIÓN CON COOKIES (MÁS SEGURO)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/AuthViews/Login";
                    options.AccessDeniedPath = "/AuthViews/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(3);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                        ? Microsoft.AspNetCore.Http.CookieSecurePolicy.None
                        : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                });

            builder.Services.AddAuthorization();

            //  CONFIGURACIÓN DE SESIONES (para datos temporales)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //  CAPAS DE LA APLICACIÓN
            builder.Services.AddPersistenceLayerIoc(builder.Configuration);
            builder.Services.AddApplicationLayerIoc();
            builder.Services.AddInfraestructureLayerIoc(builder.Configuration);
            builder.Services.Configure<LoanReminderOptions>(builder.Configuration.GetSection("LoanDueReminder"));
            builder.Services.AddHostedService<LoanDueReminderHostedService>();

            //  OUTPUT CACHE
            builder.Services.AddOutputCache();

            var app = builder.Build();

            //  MIDDLEWARE DE REDIRECCIÓN MEJORADO
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

                // Si está autenticado y accede a cualquier ruta que no sea un dashboard o API
                if (isAuthenticated &&
                    !path.StartsWithSegments("/Admin") &&
                    !path.StartsWithSegments("/UserDashboard") &&
                    !path.StartsWithSegments("/Bibliotecario") &&
                    !path.StartsWithSegments("/api") &&
                    !path.StartsWithSegments("/dev"))
                {
                    // Redirigir según su rol
                    var roleClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    var redirectPath = roleClaim switch
                    {
                        "7" => "/Admin/Dashboard",
                        "9" => "/UserDashboard/Dashboard",
                        "8" => "/Bibliotecario/Dashboard",
                        _ => "/UserDashboard/Dashboard"
                    };
                    context.Response.Redirect(redirectPath);
                    return;
                }

                await next();
            });
            //  CONFIGURACIÓN PIPELINE HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //  ORDEN CRÍTICO: Session -> Authentication -> Authorization
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();

            //  ENDPOINT DE DIAGNÓSTICO
            app.MapGet("/dev/db/ping", async (SGBLContext db) =>
            {
                var ok = await db.Database.CanConnectAsync();
                if (!ok) return Results.Problem("DB no responde");
                await db.Database.ExecuteSqlRawAsync("SELECT 1;");
                return Results.Ok(new { ok = true });
            });

            app.MapControllers();
            app.MapStaticAssets();

       
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AuthViews}/{action=Login}/{id?}")
                .WithStaticAssets();

            //favicon
            app.UseStaticFiles();

            app.Run();
        }
    }
}