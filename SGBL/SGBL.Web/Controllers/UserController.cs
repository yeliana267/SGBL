using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System.Diagnostics;

namespace SGBL.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _users;
        private readonly IRoleService _roles;
        private readonly IUserStatusService _statuses;

        public UserController(IUserService users, IRoleService roles, IUserStatusService statuses)
        {
            _users = users;
            _roles = roles;
            _statuses = statuses;
        }

        // LISTADO
        [HttpGet]
        [OutputCache(Duration = 30)]
        public async Task<IActionResult> Index()
        {
            var dtos = await _users.GetAll();
            var vms = dtos.Select(MapToVm).ToList();
            return View(vms);
        }

        // DETALLE
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _users.GetById(id);
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }

        // CREAR (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roles.GetAll();
            var statuses = await _statuses.GetAll();

            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            ViewBag.Statuses = new SelectList(statuses, "Id", "Name");

            return View(new UserViewModel());
        }

        // CREAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var created = await _users.AddAsync(MapToDto(vm));
                TempData["ok"] = $"User '{created?.Name}' creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }


        // Carga las listas para los <select>
        private async Task LoadListsAsync(int? role = null, int? status = null)
        {
            var roles = await _roles.GetAll();
            var statuses = await _statuses.GetAll();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", role);
            ViewBag.Statuses = new SelectList(statuses, "Id", "Name", status);
        }



        // EDITAR (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _users.GetById(id);
            if (dto is null) return NotFound();

            var roles = await _roles.GetAll();
            var statuses = await _statuses.GetAll();

            ViewBag.Roles = new SelectList(roles, "Id", "Name", dto.Role);
            ViewBag.Statuses = new SelectList(statuses, "Id", "Name", dto.Status);

            return View(MapToVm(dto));
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var roles = await _roles.GetAll();
                var statuses = await _statuses.GetAll();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", vm.Role);
                ViewBag.Statuses = new SelectList(statuses, "Id", "Name", vm.Status);
                return View(vm);
            }

            try
            {
                var existing = await _users.GetById(id); // <-- método que devuelva Password (hash)
                if (existing is null) return NotFound();
                var dto = MapToDto(vm);

                // Si no mandan password nuevo, conserva el actual
                if (string.IsNullOrWhiteSpace(dto.Password))
                    dto.Password = existing.Password; // el hash actual
            
                var updated = await _users.UpdateAsync(MapToDto(vm), id); // o MapToUpdateDto(vm) si usas DTO específico
                if (updated is null) return NotFound();

                TempData["ok"] = $"Usuario '{updated.Name}' actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                // Recargar listas para redisplay
                var roles = await _roles.GetAll();
                var statuses = await _statuses.GetAll();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", vm.Role);
                ViewBag.Statuses = new SelectList(statuses, "Id", "Name", vm.Status);

                return View(vm);
            }
        }


        // ELIMINAR (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _users.GetById(id);
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _users.DeleteAsync(id);
            TempData["ok"] = "Usuario eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ------------------ Helpers de mapeo ------------------
    
        private static UserViewModel MapToVm(UserDto dto) => new()
        {
            Id =dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password, // si luego vas a hashear, se hace en el servicio
            Role = dto.Role,
            Status = dto.Status
        };

        private static UserDto MapToDto(UserViewModel vm) => new()
        {
            Id = vm.Id,
            Name = vm.Name,
            Email = vm.Email,
            Role = vm.Role,
            Status = vm.Status,
            Password = vm.Password // si luego vas a hashear, se hace en el servicio
        };
    }
}
