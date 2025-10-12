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
            var dbService = new pruebaConexion(connectionString);
            dbService.ProbarConexion();
            
            
            builder.Services.AddPersistenceLayerIoc(builder.Configuration);
            builder.Services.AddApplicationLayerIoc();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
