using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    public class BookStatusController : Controller
    {
        private readonly IBookStatusService _bookStatusService;

        public BookStatusController(IBookStatusService bookStatusService)
        {
            _bookStatusService = bookStatusService;
        }

        // CAMBIA: string action → string viewAction
        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            switch (normalizedAction)
            {
                case "create":
                    return View(new BookStatusViewModel());

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _bookStatusService.GetById(id.Value);
                    if (dto is null) return NotFound();
                    return View(MapToVm(dto));

                case "index":
                default:
                    var dtos = await _bookStatusService.GetAll();
                    ViewData["BookStatusList"] = dtos.Select(MapToVm).ToList();
                    return View(new BookStatusViewModel());
            }
        }





        // CAMBIA: string action → string viewAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string viewAction, BookStatusViewModel vm)
        {
            var normalizedAction = (viewAction ?? "").ToLower();
            ViewData["Action"] = normalizedAction;

            if (!ModelState.IsValid && normalizedAction != "delete") 
            {
                return View(vm);
            }

            try
            {
                switch (normalizedAction)
                {
                    case "create":
                        var created = await _bookStatusService.AddAsync(MapToDto(vm));
                        TempData["success"] = $"Estado '{created?.Name}' creado correctamente.";
                        break;

                    case "edit":
                        var updated = await _bookStatusService.UpdateAsync(MapToDto(vm), vm.Id);
                        TempData["success"] = $"Estado '{updated?.Name}' actualizado correctamente.";
                        break;

                    case "delete":
                        Console.WriteLine($"🗑️ EJECUTANDO ELIMINACIÓN - ID: {vm.Id}");
                        await _bookStatusService.DeleteAsync(vm.Id);
                        TempData["success"] = "Estado eliminado correctamente.";
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

        private static BookStatusViewModel MapToVm(BookStatusDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        private static BookStatusDto MapToDto(BookStatusViewModel vm) => new()
        {
            Id = vm.Id,
            Name = vm.Name
        };
    }
}