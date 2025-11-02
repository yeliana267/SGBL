using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Interfaces;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")] // Solo administradores
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;

        public AdminController(
            ILogger<AdminController> logger,
            IUserService userService,
            IBookService bookService,
            IAuthorService authorService)
        {
            _logger = logger;
            _userService = userService;
            _bookService = bookService;
            _authorService = authorService;
        }

        // GET: /Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            LogAction("Accedió al Dashboard de Administrador");

            ViewData["Title"] = "Panel de Administración";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            // ⭐ OBTENER ESTADÍSTICAS EN TIEMPO REAL
            try
            {
                var users = await _userService.GetAll();
                var books = await _bookService.GetAll();
                var authors = await _authorService.GetAll();

                ViewData["TotalUsers"] = users.Count();
                ViewData["TotalBooks"] = books.Count();
                ViewData["TotalAuthors"] = authors.Count();
                ViewData["TotalLoans"] = 0; // Por ahora 0, luego puedes implementar
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estadísticas del dashboard");
                // Valores por defecto en caso de error
                ViewData["TotalUsers"] = 0;
                ViewData["TotalBooks"] = 0;
                ViewData["TotalAuthors"] = 0;
                ViewData["TotalLoans"] = 0;
            }

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