using Microsoft.EntityFrameworkCore;
using SGBL.Application;
using SGBL.Persistence;
using SGBL.Persistence.Context;
namespace SGBL.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration;
            var connectionString = config.GetConnectionString("SupabaseConnection");
            
            
            builder.Services.AddPersistenceLayerIoc(builder.Configuration);
            builder.Services.AddApplicationLayerIoc();
            // Program.cs (solo para pruebas)

            builder.Services.AddOutputCache();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var app = builder.Build();

            app.MapGet("/dev/db/ping", async (SGBLContext db) =>
            {
                var ok = await db.Database.CanConnectAsync();
                if (!ok) return Results.Problem("DB no responde");
                await db.Database.ExecuteSqlRawAsync("SELECT 1;");
                return Results.Ok(new { ok = true });
            });
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseOutputCache();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
