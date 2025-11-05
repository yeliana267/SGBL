using Microsoft.AspNetCore.Mvc;

namespace SGBL.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Si está autenticado, redirigir al dashboard según su rol
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                return role switch
                {
                    "7" => RedirectToAction("Dashboard", "Admin"),
                    "9" => RedirectToAction("Dashboard", "UserDashboard"),
                    "8" => RedirectToAction("Dashboard", "Bibliotecario"),
                    _ => RedirectToAction("Login", "AuthViews")
                };
            }

            // Si no está autenticado, redirigir al login
            return RedirectToAction("Login", "AuthViews");
        }
    }
}