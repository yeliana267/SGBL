using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")] // Solo administradores
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ILoanService _loanService;

        public AdminController(
            ILogger<AdminController> logger,
            IUserService userService,
            IBookService bookService,
            IAuthorService authorService,
            ILoanService loanService)
        {
            _logger = logger;
            _userService = userService;
            _bookService = bookService;
            _authorService = authorService;
            _loanService = loanService;
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
        public async Task<IActionResult> Reports()
        {
            LogAction("Accedió a los reportes del sistema");
            ViewData["Title"] = "Reportes del Sistema";
            ViewData["UserRole"] = CurrentUserRoleName;
            return View();

            var model = new AdminReportViewModel();

            try
            {
                var users = (await _userService.GetAll() ?? new List<UserDto>()).ToList();
                var books = (await _bookService.GetAll() ?? new List<BookDto>()).ToList();
                var loans = (await _loanService.GetAll() ?? new List<LoanDto>()).ToList();

                var today = DateTime.UtcNow.Date;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var bookTitles = books.ToDictionary(b => b.Id, b => b.Title);
                var availableCopies = books.Sum(b => b.AvailableCopies);
                var totalCopies = books.Sum(b => b.TotalCopies);

                model.RoleBreakdown = new List<ReportListItemViewModel>
                {
                    new()
                    {
                        Title = "Administradores",
                        Value = users.Count(u => u.Role == 7).ToString(),
                        Subtitle = "Rol 7"
                    },
                    new()
                    {
                        Title = "Bibliotecarios",
                        Value = users.Count(u => u.Role == 8).ToString(),
                        Subtitle = "Rol 8"
                    },
                    new()
                    {
                        Title = "Usuarios",
                        Value = users.Count(u => u.Role == 9).ToString(),
                        Subtitle = "Rol 9"
                    }
                };

                model.TopBooks = loans
                    .GroupBy(l => l.IdBook)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new ReportListItemViewModel
                    {
                        Title = bookTitles.TryGetValue(g.Key, out var title)
                            ? title
                            : $"Libro #{g.Key}",
                        Subtitle = $"Préstamos en el mes: {g.Count(l => l.DateLoan.HasValue && l.DateLoan.Value >= startOfMonth)}",
                        Value = $"{g.Count()} préstamos",
                        BadgeClass = g.Any(l => l.ReturnDate == null && l.DueDate.Date < today) ? "bg-danger" : "bg-success",
                        BadgeText = g.Any(l => l.ReturnDate == null && l.DueDate.Date < today) ? "Pendientes" : "Al día"
                    })
                    .ToList();

                model.SummaryCards = new List<ReportCountCardViewModel>
                {
                    new()
                    {
                        Title = "Usuarios registrados",
                        Value = users.Count.ToString(),
                        Icon = "fa-users",
                        ColorClass = "primary",
                        Description = "Incluye administradores, bibliotecarios y usuarios",
                    },
                    new()
                    {
                        Title = "Libros totales",
                        Value = books.Count.ToString(),
                        Icon = "fa-book",
                        ColorClass = "success",
                        Description = $"{totalCopies} copias, {availableCopies} disponibles",
                    },
                    new()
                    {
                        Title = "Préstamos este mes",
                        Value = loans.Count(l => l.DateLoan.HasValue && l.DateLoan.Value >= startOfMonth).ToString(),
                        Icon = "fa-calendar-week",
                        ColorClass = "info",
                        Description = "Movimientos generados en el mes actual",
                    },
                    new()
                    {
                        Title = "Libros en préstamo",
                        Value = (totalCopies - availableCopies).ToString(),
                        Icon = "fa-hand-holding",
                        ColorClass = "warning",
                        Description = "Copias fuera de estantería",
                    }
                };

                model.ActiveLoans = loans.Count(l => l.ReturnDate == null);
                model.OverdueLoans = loans.Count(l => l.ReturnDate == null && l.DueDate.Date < today);
                model.ReturnedThisMonth = loans.Count(l => l.ReturnDate.HasValue && l.ReturnDate.Value >= startOfMonth);
                model.MonthlyLoans = loans.Count(l => l.DateLoan.HasValue && l.DateLoan.Value >= startOfMonth);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al construir el modelo de reportes de administrador");
            }

            return View(model);
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
