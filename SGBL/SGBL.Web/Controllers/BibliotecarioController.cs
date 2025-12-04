using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Application.ViewModels;

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
                .Count(l => l.Status == 2);
            // Pretamos pendiente a activacion
            ViewBag.pending = loans
               .Count(l => l.Status == 1);

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

    // GET: /Bibliotecario/Reports
    [HttpGet]
    public async Task<IActionResult> Reports()
    {
        LogAction("Accedió a los reportes de circulación");

        ViewData["Title"] = "Reportes de circulación";
        ViewData["UserRole"] = CurrentUserRoleName;
        ViewData["UserName"] = CurrentUserName;
        ViewData["UserEmail"] = CurrentUserEmail;

        var loans = (await _loanService.GetAll()).ToList();

        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        var upcomingReturns = loans
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .Take(6)
            .Select(l => new ReportListItemViewModel
            {
                Title = l.BookTitle ?? $"Libro #{l.IdBook}",
                Subtitle = l.UserName ?? $"Usuario #{l.IdUser}",
                Value = l.DueDate.ToString("dd MMM"),
                BadgeClass = l.DueDate.Date < today ? "bg-danger" : "bg-primary",
                BadgeText = l.DueDate.Date < today
                    ? $"Atraso: {(today - l.DueDate.Date).Days} días"
                    : $"Faltan {(l.DueDate.Date - today).Days} días"
            })
            .ToList();

        var pendingPickups = loans
            .Where(l => (l.Status ?? 0) == 1 && l.ReturnDate == null)
            .OrderBy(l => l.PickupDeadline)
            .Take(6)
            .Select(l => new ReportListItemViewModel
            {
                Title = l.BookTitle ?? $"Libro #{l.IdBook}",
                Subtitle = l.UserName ?? $"Usuario #{l.IdUser}",
                Value = l.PickupDeadline.ToString("dd MMM"),
                BadgeClass = l.PickupDeadline.Date < today ? "bg-danger" : "bg-warning",
                BadgeText = l.PickupDeadline.Date < today ? "Vencido" : "Pendiente"
            })
            .ToList();

        var model = new LibrarianReportViewModel
        {
            SummaryCards = new List<ReportCountCardViewModel>
                {
                    new()
                    {
                        Title = "Préstamos activos",
                        Value = loans.Count(l => l.ReturnDate == null && ((l.Status ?? 0) == 1 || (l.Status ?? 0) == 2)).ToString(),
                        Icon = "fa-book-reader",
                        ColorClass = "primary",
                        Description = "Pendientes y recogidos"
                    },
                    new()
                    {
                        Title = "Devoluciones atrasadas",
                        Value = loans.Count(l => l.ReturnDate == null && l.DueDate.Date < today).ToString(),
                        Icon = "fa-clock",
                        ColorClass = "danger",
                        Description = "Fecha de entrega vencida"
                    },
                    new()
                    {
                        Title = "Devoluciones hoy",
                        Value = loans.Count(l => l.ReturnDate.HasValue && l.ReturnDate.Value.Date == today).ToString(),
                        Icon = "fa-undo",
                        ColorClass = "success",
                        Description = "Recibidas en la fecha"
                    },
                    new()
                    {
                        Title = "Préstamos del mes",
                        Value = loans.Count(l => l.DateLoan.HasValue && l.DateLoan.Value >= startOfMonth).ToString(),
                        Icon = "fa-calendar-plus",
                        ColorClass = "info",
                        Description = "Movimientos generados este mes"
                    }
                },
            UpcomingReturns = upcomingReturns,
            PendingPickups = pendingPickups
        };

        return View(model);
    }
    }
}