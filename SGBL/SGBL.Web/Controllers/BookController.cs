using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IBookStatusService _bookStatusService; // 👈 Agrega este servicio

        public BookController(IBookService bookService, IBookStatusService bookStatusService)
        {
            _bookService = bookService;
            _bookStatusService = bookStatusService; // 👈 Inyecta el servicio
        }





        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            // 👇 CARGAR ESTADOS DISPONIBLES PARA TODOS LOS CASOS
            var statusDtos = await _bookStatusService.GetAll();
            var availableStatuses = statusDtos.Select(s => new BookStatusViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            switch (normalizedAction)
            {
                case "create":
                    var createVm = new BookViewModel { AvailableStatuses = availableStatuses };
                    return View(createVm);

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _bookService.GetById(id.Value);
                    if (dto is null) return NotFound();

                    var vm = MapToVm(dto);
                    vm.AvailableStatuses = availableStatuses;
                    return View(vm);

                case "index":
                default:
                    var dtos = await _bookService.GetAll();
                    var bookList = dtos.Select(MapToVm).ToList();

                    // 👇 ASIGNAR LOS ESTADOS A CADA LIBRO EN EL LISTADO
                    foreach (var book in bookList)
                    {
                        book.AvailableStatuses = availableStatuses;
                    }

                    ViewData["BookList"] = bookList;
                    return View(new BookViewModel { AvailableStatuses = availableStatuses });
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
                        TempData["success"] = $"Libro '{created?.Title}' creado correctamente.";
                        break;

                    case "edit":
                        var updated = await _bookService.UpdateAsync(MapToDto(vm), vm.Id);
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
            StatusId = dto.StatusId, // 👈 Usa StatusId
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
            StatusId = vm.StatusId // 👈 Usa StatusId
        };
    }
}