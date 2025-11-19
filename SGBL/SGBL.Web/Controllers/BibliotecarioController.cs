using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Interfaces;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7,8")] // ✅ Solo usuarios con rol 7 (Admin) O 8 (Bibliotecario)
    public class BibliotecarioController : BaseController
    {
        private readonly ILogger<BibliotecarioController> _logger;
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        public BibliotecarioController(
         ILogger<BibliotecarioController> logger,
         ILoanService loanService,
         IBookService bookService
     )
        {
            _logger = logger;
            _loanService = loanService;
            _bookService = bookService;
        }

        // GET: /Bibliotecario/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            LogAction("Accedió al Dashboard de Bibliotecario");

            ViewData["Title"] = "Dashboard Bibliotecario";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            // 🔹 Consultas simples
            var today = DateTime.UtcNow.Date;

            var loans = await _loanService.GetAll();
            var books = await _bookService.GetAll();

            // Préstamos creados hoy
            ViewBag.LoansToday = loans
                .Count(l => l.DateLoan.HasValue && l.DateLoan.Value.Date == today);

            // Devoluciones pendientes (vencidos y no devueltos)
            ViewBag.PendingReturns = loans
                .Count(l => l.ReturnDate == null && l.DueDate.Date < today);

            // Libros disponibles (suma de copias disponibles)
            ViewBag.AvailableBooks = books.Sum(b => b.AvailableCopies);

            // Préstamos activos: Pendiente (1) o Recogido (2)
            ViewBag.ActiveLoans = loans
                .Count(l =>  l.Status == 2);
            // Pretamos pendiente a activacion
            ViewBag.pending = loans
               .Count(l => l.Status==1);

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