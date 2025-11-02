using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7,8")] // ✅ Solo usuarios con rol 7 (Admin) O 8 (Bibliotecario)
    public class BibliotecarioController : BaseController
    {
        private readonly ILogger<BibliotecarioController> _logger;

        public BibliotecarioController(ILogger<BibliotecarioController> logger)
        {
            _logger = logger;
        }

        // GET: /Bibliotecario/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            LogAction("Accedió al Dashboard de Bibliotecario");

            ViewData["Title"] = "Dashboard Bibliotecario";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            return View();
        }

        // GET: /Bibliotecario/LoanManagement
        [HttpGet]
        public IActionResult LoanManagement()
        {
            LogAction("Accedió a la gestión de préstamos");

            ViewData["Title"] = "Gestión de Préstamos";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /Bibliotecario/BookManagement
        [HttpGet]
        public IActionResult BookManagement()
        {
            LogAction("Accedió a la gestión de libros");

            ViewData["Title"] = "Gestión de Libros";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        // GET: /Bibliotecario/Returns
        [HttpGet]
        public IActionResult Returns()
        {
            LogAction("Accedió a la gestión de devoluciones");

            ViewData["Title"] = "Gestión de Devoluciones";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }
    }
}