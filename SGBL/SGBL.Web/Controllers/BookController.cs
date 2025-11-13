using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IBookStatusService _bookStatusService;
        private readonly IAuthorService _authorService; // ⭐ NUEVO SERVICIO
        private readonly IGenreService _genreService; // ⭐ NUEVO SERVICIO
      // ⭐ NUEVO SERVICIO


        public BookController(
            IBookService bookService,
            IBookStatusService bookStatusService,
            IAuthorService authorService,
            IGenreService genreService
            ) // ⭐ AGREGAR PARÁMETRO
        {
            _bookService = bookService;
            _bookStatusService = bookStatusService;
            _authorService = authorService; // ⭐ INICIALIZAR
            _genreService = genreService;
           
        }

        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            // CARGAR ESTADOS DISPONIBLES PARA TODOS LOS CASOS
            var statusDtos = await _bookStatusService.GetAll();
            var availableStatuses = statusDtos.Select(s => new BookStatusViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

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
            switch (normalizedAction)
            {
                case "create":
                    var createVm = new BookViewModel
                    {
                        AvailableStatuses = availableStatuses,
                        AvailableAuthors = availableAuthors,
                        AvailableGenres = availableGenres 
                    };
                    return View(createVm);

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _bookService.GetById(id.Value);
                    if (dto is null) return NotFound();

                    var vm = MapToVm(dto);
                    vm.AvailableStatuses = availableStatuses;
                    vm.AvailableAuthors = availableAuthors; // ⭐ AGREGAR AUTORES
                    vm.AvailableGenres = availableGenres; // AGREGAR GENEROS

                    // ⭐ CARGAR AUTORES y generos ACTUALES DEL LIBRO
                    if (id.HasValue && (normalizedAction == "edit" || normalizedAction == "details"))
                    {
                        var bookAuthors = await _bookService.GetBookAuthors(id.Value);
                        vm.CurrentAuthors = bookAuthors.Select(a => new AuthorViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();
                        vm.SelectedAuthorIds = bookAuthors.Select(a => a.Id).ToList();


                        var bookGenres = await _bookService.GetBookGenres(id.Value);
                        vm.CurrentGenres = bookGenres.Select(a => new GenreViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();
                        vm.SelectedGenresIds = bookGenres.Select(a => a.Id).ToList();
                    }


                    return View(vm);

                case "index":
                default:
                    var dtos = await _bookService.GetAll();
                    var bookList = dtos.Select(MapToVm).ToList();

                    // ASIGNAR LOS ESTADOS, generos Y AUTORES A CADA LIBRO EN EL LISTADO
                    foreach (var book in bookList)
                    {
                        book.AvailableStatuses = availableStatuses;
                        // ⭐ CARGAR AUTORES PARA CADA LIBRO EN EL LISTADO
                        var bookAuthors = await _bookService.GetBookAuthors(book.Id);
                        book.CurrentAuthors = bookAuthors.Select(a => new AuthorViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();
                        // ⭐ CARGAR generos PARA CADA LIBRO EN EL LISTADO
                        var bookGenres = await _bookService.GetBookGenres(book.Id);
                        book.CurrentGenres = bookGenres.Select(a => new GenreViewModel
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList();
                    }

                    ViewData["BookList"] = bookList;
                    return View(new BookViewModel
                    {
                        AvailableStatuses = availableStatuses,
                        AvailableAuthors = availableAuthors, // ⭐ AGREGAR AUTORES
                        AvailableGenres = availableGenres
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string viewAction, BookViewModel vm)
        {
            var normalizedAction = (viewAction ?? "").ToLower();
            ViewData["Action"] = normalizedAction;

            // Cargar estados disponibles en caso de error
            var statusDtos = await _bookStatusService.GetAll();
            vm.AvailableStatuses = statusDtos.Select(s => new BookStatusViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            // ⭐ CARGAR AUTORES DISPONIBLES EN CASO DE ERROR
            var authorDtos = await _authorService.GetAll();
            vm.AvailableAuthors = authorDtos.Select(a => new AuthorViewModel
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
            // ⭐ CARGAR generos DISPONIBLES EN CASO DE ERROR
            var genreDtos = await _genreService.GetAll();
            vm.AvailableGenres = genreDtos.Select(a => new GenreViewModel
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            if (!ModelState.IsValid && normalizedAction != "delete")
            {
                return View(vm);
            }

            try
            {
                switch (normalizedAction)
                {
                    case "create":
                        var created = await _bookService.AddAsync(MapToDto(vm));

                        // ⭐ GUARDAR LAS RELACIONES CON AUTORES
                        if (vm.SelectedAuthorIds != null && vm.SelectedAuthorIds.Any())
                        {
                            await _bookService.AddAuthorsToBook(created.Id, vm.SelectedAuthorIds);
                        }
                        // ⭐ GUARDAR LAS RELACIONES CON generos
                        if (vm.SelectedGenresIds != null && vm.SelectedGenresIds.Any())
                        {
                            await _bookService.AddGenresToBook(created.Id, vm.SelectedGenresIds);
                        }

                        TempData["success"] = $"Libro '{created?.Title}' creado correctamente.";
                        break;

                    case "edit":
                        var updated = await _bookService.UpdateAsync(MapToDto(vm), vm.Id);

                        // ⭐ ACTUALIZAR LAS RELACIONES CON AUTORES
                        if (vm.SelectedAuthorIds != null && vm.SelectedAuthorIds.Any())
                        {
                            await _bookService.UpdateBookAuthors(vm.Id, vm.SelectedAuthorIds);
                        }
                        else
                        {
                            // Si no se seleccionaron autores, eliminar relaciones existentes
                            await _bookService.UpdateBookAuthors(vm.Id, new List<int>());
                        }

                        // ⭐ ACTUALIZAR LAS RELACIONES CON generos
                        if (vm.SelectedGenresIds != null && vm.SelectedGenresIds.Any())
                        {
                            await _bookService.UpdateBookGenres(vm.Id, vm.SelectedGenresIds);
                        }
                        else
                        {
                            // Si no se seleccionaron autores, eliminar relaciones existentes
                            await _bookService.UpdateBookGenres(vm.Id, new List<int>());
                        }

                        TempData["success"] = $"Libro '{updated?.Title}' actualizado correctamente.";
                        break;

                    case "delete":
                        await _bookService.DeleteAsync(vm.Id);
                        TempData["success"] = "Libro eliminado correctamente.";
                        break;

                    default:
                        TempData["error"] = "Acción no válida";
                        return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Action"] = normalizedAction;
                return View(vm);
            }
        }

        public async Task DecreaseAvailableCopies(int bookId)
        {

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

        private static BookDto MapToDto(BookViewModel vm) => new()
        {
            Id = vm.Id,
            Title = vm.Title,
            Isbn = vm.Isbn,
            Description = vm.Description,
            PublicationYear = vm.PublicationYear,
            Pages = vm.Pages,
            TotalCopies = vm.TotalCopies,
            AvailableCopies = vm.AvailableCopies,
            Ubication = vm.Ubication,
            StatusId = vm.StatusId
        };
    }
}