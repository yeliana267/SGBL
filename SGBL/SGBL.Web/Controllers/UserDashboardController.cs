using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "9")] // Solo usuarios con rol 2 (User) pueden acceder
    public class UserDashboardController : BaseController
    {
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(ILogger<UserDashboardController> logger)
        {
            _logger = logger;
        }

        // GET: /UserDashboard/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            LogAction("Accedió al Dashboard de Usuario");

            ViewData["Title"] = "Mi Dashboard";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            return View();
        }

        // GET: /UserDashboard/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            LogAction("Accedió a su perfil de usuario");

            ViewData["Title"] = "Mi Perfil";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /UserDashboard/Books
        [HttpGet]
        public IActionResult Books()
        {
            LogAction("Accedió a la búsqueda de libros");

            ViewData["Title"] = "Buscar Libros";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /UserDashboard/MyLoans
        [HttpGet]
        public IActionResult MyLoans()
        {
            LogAction("Accedió a sus préstamos");

            ViewData["Title"] = "Mis Préstamos";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }
    }
}