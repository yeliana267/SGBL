using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System.Security.Claims;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "9")]
    public class UserDashboardController : BaseController
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;

        public UserDashboardController(
            IBookService bookService,
            IAuthorService authorService,
            IGenreService genreService,
            ILoanService loanService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
        }

        private int GetCurrentUserId()
        {
            var rawId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(rawId))
                throw new Exception("No se pudo obtener el ID del usuario actual.");

            return int.Parse(rawId);
        }

        // GET: /UserDashboard/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            LogAction("Accedió al Dashboard de Usuario");

            ViewData["Title"] = "Mi Dashboard";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            var userId = GetCurrentUserId();

            // 1. Traer todos los préstamos del usuario
            var loans = await _loanService.GetLoansByUserAsync(userId);
            var loansList = loans.ToList();

            var today = DateTime.UtcNow.Date;

            // Activos: sin fecha de devolución (pendiente o recogido)
            var activeLoans = loansList
                .Where(l => l.ReturnDate == null)
                .ToList();

            // Leídos: devueltos (ReturnDate != null o estado 3)
            var readLoans = loansList
                .Where(l => l.ReturnDate != null || l.Status == 3)
                .ToList();

            // 2. Diccionario de libros para mostrar títulos
            var allBooks = await _bookService.GetAll();
            var bookDict = allBooks.ToDictionary(b => b.Id, b => b.Title);

            // 3. Construir lista de préstamos activos resumidos
            var activeLoanSummaries = activeLoans
                .Select(l =>
                {
                    var title = bookDict.TryGetValue(l.IdBook, out var t)
                        ? t
                        : $"Libro #{l.IdBook}";

                    var due = l.DueDate.Date;
                    var daysRemaining = (due - today).Days;

                    return new UserLoanSummaryViewModel
                    {
                        LoanId = l.Id,
                        BookTitle = title,
                        DateLoan = l.DateLoan,
                        DueDate = l.DueDate,
                        DaysRemaining = daysRemaining
                    };
                })
                .OrderBy(l => l.DueDate)
                .ToList();

            // 4. Próxima devolución
            var nextDue = activeLoanSummaries.FirstOrDefault();

            var model = new UserDashboardViewModel
            {
                UserName = CurrentUserName ?? "Usuario",
                ActiveLoansCount = activeLoans.Count,
                ReadBooksCount = readLoans
                    .Select(l => l.IdBook)
                    .Distinct()
                    .Count(),
                NextDueBookTitle = nextDue?.BookTitle,
                NextDueInDays = nextDue?.DaysRemaining,
                ActiveLoans = activeLoanSummaries
            };

            return View(model);
        }

        // GET: /UserDashboard/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            LogAction("Accedió a su perfil de usuario");

            ViewData["Title"] = "Mi Perfil";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            // estadísticas simples para el usuario
            var userId = GetCurrentUserId();
            var loans = await _loanService.GetLoansByUserAsync(userId);

            ViewBag.ActiveLoans = loans.Count(l =>
                l.Status == 1 || l.Status == 2
            );

            ViewBag.ReturnedLoans = loans.Count(l =>
                l.Status == 3
            );


            return View();
        }


        // GET: /UserDashboard/Books
        [HttpGet]
        public async Task<IActionResult> Books(
            string viewAction = "search",
            int? id = null,
            string? title = null,
            int? genreId = null,
            int? authorId = null,
            int? bookId = null,
            int pageNumber = 1,
            int pageSize = 3 // cambien este numero para que salgan mas o menos libros por pagina
        )
        {
            // Información del usuario
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;
            ViewData["UserRole"] = CurrentUserRoleName;

            var authorDtos = await _authorService.GetAll();
            var availableAuthors = authorDtos.Select(a => new AuthorViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography
            }).ToList();

            var generosDtos = await _genreService.GetAll();
            var availableGenres = generosDtos.Select(s => new GenreViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            }).ToList();

            ViewBag.AvailableAuthors = availableAuthors;
            ViewBag.AvailableGenres = availableGenres;
            ViewBag.TitleFilter = title;
            ViewBag.GenreIdFilter = genreId;
            ViewBag.AuthorIdFilter = authorId;

            var normalizedAction = (viewAction ?? "search").ToLower();
            ViewData["Action"] = normalizedAction;

            switch (normalizedAction)
            {
                // 🔍 SEARCH
                #region SearchBooks
                case "search":
                default:
                    var pagedResultDto = await _bookService.SearchBooksPagedAsync(
                        title, genreId, authorId, pageNumber, pageSize);

                    var vm = new PagedResultViewModel<BookViewModel>
                    {
                        Items = pagedResultDto.Items.Select(MapToVm).ToList(),
                        TotalCount = pagedResultDto.TotalItems,
                        PageNumber = pagedResultDto.PageNumber,
                        PageSize = pagedResultDto.PageSize
                    };

                    if (title == null)
                    {
                        if (authorId == null)
                        {
                            ViewBag.SelectedTitle = availableGenres
                                .Where(a => a.Id == genreId)
                                .Select(a => a.Name)
                                .FirstOrDefault();

                            ViewBag.SelectedMensaje = availableGenres
                                .Where(a => a.Id == genreId)
                                .Select(a => a.Description)
                                .FirstOrDefault();
                        }
                        else
                        {
                            ViewBag.SelectedTitle = availableAuthors
                                .Where(a => a.Id == authorId)
                                .Select(a => a.Name)
                                .FirstOrDefault();

                            ViewBag.SelectedMensaje = availableAuthors
                                .Where(a => a.Id == authorId)
                                .Select(a => a.Biography)
                                .FirstOrDefault();
                        }
                    }

                    return View(vm);
                #endregion

                #region ShowBookDetails
                case "showbook":
                    if (!bookId.HasValue)
                    {
                        return BadRequest("Debe especificar el libro a mostrar.");
                    }
                    else
                    {
                        BookViewModel newBook = MapToVm(await _bookService.GetById(bookId.Value));

                        var bookAuthors = await _bookService.GetBookAuthors(bookId.Value);
                        newBook.CurrentAuthors = bookAuthors.Select(a => new AuthorViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();

                        var bookGenres = await _bookService.GetBookGenres(bookId.Value);
                        newBook.CurrentGenres = bookGenres.Select(a => new GenreViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();

                        PagedResultViewModel<BookViewModel> vmPage = new PagedResultViewModel<BookViewModel>();
                        vmPage.Items.Add(newBook);

                        return View(vmPage);
                    }
                    #endregion
            }
        }

        // GET: /UserDashboard/MyLoans
        [HttpGet]
        public IActionResult MyLoans()
        {
            LogAction("Accedió a sus préstamos");

            // Ya tienes toda la lógica de préstamos en LoanController.Index
            // y ahí filtras por usuario, así que simplemente redirigimos.
            return RedirectToAction("Index", "Loan");
        }

        private static BookViewModel MapToVm(BookDto dto) => new()
        {
            Id = dto.Id,
            Title = dto.Title,
            Isbn = dto.Isbn,
            Description = dto.Description,
            PublicationYear = dto.PublicationYear,
            Pages = dto.Pages,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.AvailableCopies,
            Ubication = dto.Ubication,
            StatusId = dto.StatusId,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
