using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")] // Solo usuarios con rol 1 (Admin) pueden acceder
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        // GET: /Admin/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            LogAction("Accedió al Dashboard de Administrador");

            ViewData["Title"] = "Dashboard Administrador";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            return View();
        }

        // GET: /Admin/Users
        [HttpGet]
        public IActionResult Users()
        {
            LogAction("Accedió a la gestión de usuarios");

            ViewData["Title"] = "Gestión de Usuarios";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /Admin/Reports
        [HttpGet]
        public IActionResult Reports()
        {
            LogAction("Accedió a los reportes del sistema");

            ViewData["Title"] = "Reportes del Sistema";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /Admin/Settings
        [HttpGet]
        public IActionResult Settings()
        {
            LogAction("Accedió a la configuración del sistema");

            ViewData["Title"] = "Configuración del Sistema";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }
    }
}