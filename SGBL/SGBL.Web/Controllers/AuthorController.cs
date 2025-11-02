using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Author;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")] // Solo administradores
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;
        private readonly INationalityService _nationalityService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(
            IAuthorService authorService,
            INationalityService nationalityService,
            ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _nationalityService = nationalityService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            LogAction("Accedió a la gestión de autores");

            ViewData["Title"] = "Gestión de Autores";
            ViewData["UserRole"] = CurrentUserRoleName;
            ViewData["UserName"] = CurrentUserName;
            ViewData["UserEmail"] = CurrentUserEmail;

            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            // Cargar nacionalidades disponibles
            var nationalityDtos = await _nationalityService.GetAll();
            var availableNationalities = nationalityDtos.Select(n => new NationalityViewModel
            {
                Id = n.Id,
                Name = n.Name
            }).ToList();

            switch (normalizedAction)
            {
                case "create":
                    var createVm = new AuthorViewModel
                    {
                        Nationalities = availableNationalities,
                        BirthDate = DateTime.Now.AddYears(-30)
                    };
                    return View(createVm); // ⭐ CAMBIO: Sin ruta específica

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _authorService.GetById(id.Value);
                    if (dto is null) return NotFound();

                    var vm = MapToVm(dto);
                    vm.Nationalities = availableNationalities;
                    return View(vm); // ⭐ CAMBIO: Sin ruta específica

                case "index":
                default:
                    var dtos = await _authorService.GetAll();
                    var authorList = dtos.Select(MapToVm).ToList();

                    foreach (var author in authorList)
                    {
                        author.Nationalities = availableNationalities;
                    }

                    ViewData["AuthorList"] = authorList;

                    var viewModel = new AuthorViewModel { Nationalities = availableNationalities };
                    return View(viewModel); // ⭐ CAMBIO: Sin ruta específica
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string viewAction, AuthorViewModel vm)
        {
            // ⭐⭐ IGNORAR LA VALIDACIÓN DE NationalityName - NO EXISTE EN EL MODELO ⭐⭐
            ModelState.Remove("NationalityName");

            var normalizedAction = (viewAction ?? "").ToLower();
            ViewData["Action"] = normalizedAction;

            // Cargar nacionalidades en caso de error
            var nationalityDtos = await _nationalityService.GetAll();
            vm.Nationalities = nationalityDtos.Select(n => new NationalityViewModel
            {
                Id = n.Id,
                Name = n.Name
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
                        var created = await _authorService.AddAsync(MapToDto(vm));
                        LogAction($"Creó el autor: {created?.Name}");
                        TempData["success"] = $"Autor '{created?.Name}' creado correctamente.";
                        break;

                    case "edit":
                        var updated = await _authorService.UpdateAsync(MapToDto(vm), vm.Id);
                        LogAction($"Actualizó el autor: {updated?.Name}");
                        TempData["success"] = $"Autor '{updated?.Name}' actualizado correctamente.";
                        break;

                    case "delete":
                        await _authorService.DeleteAsync(vm.Id);
                        LogAction($"Eliminó el autor ID: {vm.Id}");
                        TempData["success"] = "Autor eliminado correctamente.";
                        break;

                    default:
                        TempData["error"] = "Acción no válida";
                        return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogAction($"Error en acción {normalizedAction}: {ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Action"] = normalizedAction;
                return View(vm);
            }
        }
        private static AuthorViewModel MapToVm(AuthorDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Biography = dto.Biography,
            BirthDate = dto.BirthDate,
            Nationality = dto.Nationality,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        private static AuthorDto MapToDto(AuthorViewModel vm) => new()
        {
            Id = vm.Id,
            Name = vm.Name,
            Biography = vm.Biography,
            BirthDate = vm.BirthDate, 
            Nationality = vm.Nationality,
       
            CreatedAt = DateTime.UtcNow, 
            UpdatedAt = DateTime.UtcNow  
        };
    }
}