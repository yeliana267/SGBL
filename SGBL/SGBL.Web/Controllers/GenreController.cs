using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]

    public class GenreController : Controller
    {

        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // CAMBIA: string action → string viewAction
        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            switch (normalizedAction)
            {
                case "create":
                    return View(new GenreViewModel());

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _genreService.GetById(id.Value);
                    if (dto is null) return NotFound();
                    return View(MapToVm(dto));

                case "index":
                default:
                    var dtos = await _genreService.GetAll();
                    ViewData["GenreList"] = dtos.Select(MapToVm).ToList();
                    return View(new GenreViewModel());
            }
        }





        // CAMBIA: string action → string viewAction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string viewAction, GenreViewModel vm)
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
                        var created = await _genreService.AddAsync(MapToDto(vm));
                        TempData["success"] = $"Genero '{created?.Name}' creado correctamente.";
                        break;

                    case "edit":
                        var updated = await _genreService.UpdateAsync(MapToDto(vm), vm.Id);
                        TempData["success"] = $"Genero '{updated?.Name}' actualizado correctamente.";
                        break;

                    case "delete":
                        Console.WriteLine($"🗑️ EJECUTANDO ELIMINACIÓN - ID: {vm.Id}");
                        await _genreService.DeleteAsync(vm.Id);
                        TempData["success"] = "Genero eliminado correctamente.";
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

        private static GenreViewModel MapToVm(GenreDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        private static GenreDto MapToDto(GenreViewModel vm) => new()
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description
        };
    }
}