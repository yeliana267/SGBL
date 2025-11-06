using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "9")] // Solo usuarios con rol 2 (User) pueden acceder
    public class UserDashboardController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService; // ⭐ NUEVO SERVICIO
        private readonly IGenreService _genreService; // ⭐ NUEVO SERVICIO
                                                      // ⭐ NUEVO SERVICIO


        public UserDashboardController(
            IBookService bookService,
            IAuthorService authorService,
            IGenreService genreService
            ) // ⭐ AGREGAR PARÁMETRO
        {
            _bookService = bookService;
            _authorService = authorService; // ⭐ INICIALIZAR
            _genreService = genreService;

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
        public async Task<IActionResult> Books(string? title, int? genreId, int? authorId, int pageNumber = 1, int pageSize = 10)

        {
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;
            ViewData["UserRole"] = CurrentUserRoleName;


            var pagedResultDto = await _bookService.SearchBooksPagedAsync(title, genreId, authorId, pageNumber, pageSize);

            var vm = new PagedResultViewModel
            {
                Items = pagedResultDto.Items.Select(MapToVm).ToList(),
                TotalItems = pagedResultDto.TotalItems,
                PageNumber = pagedResultDto.PageNumber,
                PageSize = pagedResultDto.PageSize,
                TotalPages = pagedResultDto.TotalPages
            };

            // ⭐ CARGAR AUTORES DISPONIBLES PARA TODOS LOS CASOS
            var authorDtos = await _authorService.GetAll();
            var availableAuthors = authorDtos.Select(a => new AuthorViewModel
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
            // CARGAR generos DISPONIBLES PARA TODOS LOS CASOS
            var generosDtos = await _genreService.GetAll();
            var availableGenres = generosDtos.Select(s => new GenreViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
            // Guardamos los filtros en ViewBag
            ViewBag.AvailableAuthors = availableAuthors;
            ViewBag.AvailableGenres = availableGenres;
            ViewBag.TitleFilter = title;
            ViewBag.GenreIdFilter = genreId;
            ViewBag.AuthorIdFilter = authorId;

            return View(vm);
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