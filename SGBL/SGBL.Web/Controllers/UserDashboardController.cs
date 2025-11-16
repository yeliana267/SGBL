using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "9")]
    public class UserDashboardController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly ILoanService _loanService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(
            IBookService bookService,
            IAuthorService authorService,
            IGenreService genreService,
            ILoanService loanService,
            INotificationService notificationService,
            ILogger<UserDashboardController> logger)
        {
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _loanService = loanService;
            _notificationService = notificationService;
            _logger = logger;
        }

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

        [HttpGet]
        public IActionResult Profile()
        {
            LogAction("Accedió a su perfil de usuario");

            ViewData["Title"] = "Mi Perfil";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Books(
            string viewAction = "search",
            int? id = null,
            string? title = null,
            int? genreId = null,
            int? authorId = null,
            int? bookId = null,
            int pageNumber = 1,
            int pageSize = 3)
        {
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

            var genreDtos = await _genreService.GetAll();
            var availableGenres = genreDtos.Select(s => new GenreViewModel
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
                case "search":
                default:
                    var pagedResultDto = await _bookService.SearchBooksPagedAsync(title, genreId, authorId, pageNumber, pageSize);

                    var vm = new PagedResultViewModel
                    {
                        Items = pagedResultDto.Items.Select(MapToVm).ToList(),
                        TotalItems = pagedResultDto.TotalItems,
                        PageNumber = pagedResultDto.PageNumber,
                        PageSize = pagedResultDto.PageSize,
                        TotalPages = pagedResultDto.TotalPages
                    };

                    if (title == null)
                    {
                        if (authorId == null)
                        {
                            ViewBag.SelectedTitle = availableGenres.Where(a => a.Id == genreId).Select(a => a.Name).FirstOrDefault();
                            ViewBag.SelectedMensaje = availableGenres.Where(a => a.Id == genreId).Select(a => a.Description).FirstOrDefault();
                        }
                        else
                        {
                            ViewBag.SelectedTitle = availableAuthors.Where(a => a.Id == authorId).Select(a => a.Name).FirstOrDefault();
                            ViewBag.SelectedMensaje = availableAuthors.Where(a => a.Id == authorId).Select(a => a.Biography).FirstOrDefault();
                        }
                    }

                    return View(vm);

                case "showbook":
                    if (!bookId.HasValue)
                    {
                        return BadRequest("Debe especificar el libro a mostrar.");
                    }

                    var bookVm = MapToVm(await _bookService.GetById(bookId.Value));
                    var bookAuthors = await _bookService.GetBookAuthors(bookId.Value);
                    bookVm.CurrentAuthors = bookAuthors.Select(a => new AuthorViewModel
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList();

                    var bookGenres = await _bookService.GetBookGenres(bookId.Value);
                    bookVm.CurrentGenres = bookGenres.Select(a => new GenreViewModel
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList();

                    var singleItemVm = new PagedResultViewModel();
                    singleItemVm.Items.Add(bookVm);
                    return View(singleItemVm);
            }

            return View(new PagedResultViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLoan(int bookId)
        {
            if (bookId <= 0)
            {
                TempData["LoanError"] = "El libro seleccionado no es válido.";
                return RedirectToAction(nameof(Books));
            }

            try
            {
                var book = await _bookService.GetById(bookId);
                if (book == null)
                {
                    TempData["LoanError"] = "No se encontró el libro solicitado.";
                    return RedirectToAction(nameof(Books));
                }

                if (book.AvailableCopies <= 0)
                {
                    TempData["LoanError"] = "Este libro no tiene copias disponibles por el momento.";
                    return RedirectToAction(nameof(Books), new { viewAction = "showbook", bookId });
                }

                var hasActiveLoan = await _loanService.UserHasActiveLoanAsync(CurrentUserId, bookId);
                if (hasActiveLoan)
                {
                    TempData["LoanError"] = "Ya tienes un préstamo activo para este libro.";
                    return RedirectToAction(nameof(Books), new { viewAction = "showbook", bookId });
                }

                var now = DateTime.UtcNow;

                var loan = new LoanDto
                {
                    IdBook = bookId,
                    IdUser = CurrentUserId,
                    IdLibrarian = null,
                    DateLoan = now,
                    DueDate = now.AddDays(7),
                    PickupDeadline = now.AddDays(1),
                    Status = 1,
                    FineAmount = 0,
                    Notes = $"Solicitud creada por el usuario {CurrentUserName}"
                };

                var createdLoan = await _loanService.AddAsync(loan);
                if (createdLoan == null)
                {
                    TempData["LoanError"] = "No se pudo registrar el préstamo. Intenta más tarde.";
                    return RedirectToAction(nameof(Books), new { viewAction = "showbook", bookId });
                }

                await _bookService.DecreaseAvailableCopies(bookId);

                var notification = new NotificationDto
                {
                    IdUser = CurrentUserId,
                    IdBook = bookId,
                    IdLoan = createdLoan.Id,
                    Title = $"Préstamo solicitado: {book.Title}",
                    Message = $"Tu préstamo vence el {createdLoan.DueDate:dd/MM/yyyy}.",
                    Status = 1,
                    Type = 1,
                    ReadDate = null
                };

                await _notificationService.AddAsync(notification);
                TempData["LoanSuccess"] = $"Tu préstamo para \"{book.Title}\" fue registrado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al solicitar el préstamo del libro {BookId} por el usuario {UserId}", bookId, CurrentUserId);
                TempData["LoanError"] = "Ocurrió un error al registrar el préstamo.";
            }

            return RedirectToAction(nameof(Books), new { viewAction = "showbook", bookId });
        }

        [HttpGet]
        public IActionResult MyLoans()
        {
            LogAction("Accedió a sus préstamos");

            ViewData["Title"] = "Mis Préstamos";
            ViewData["UserRole"] = CurrentUserRoleName;

            return View();
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
