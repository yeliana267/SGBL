using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Application.ViewModels;
using System.Diagnostics;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]
    public class UserStatusController : Controller
    {
        private readonly IUserStatusService _userStatusService;

            public UserStatusController(IUserStatusService userStatusService)
            {
                _userStatusService = userStatusService;
            }

        // LISTADO
        [HttpGet]
        [OutputCache(Duration = 30)]
        public async Task<IActionResult> Index()
        {
            var dtos = await _userStatusService.GetAll(); // ya AsNoTracking en repo
            var vms = dtos.Select(MapToVm).ToList();
            return View(vms);
        }


        // DETALLE
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _userStatusService.GetById(id);
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }

            // CREAR (GET)
            public IActionResult Create()
            {
                return View(new UserStatusViewModel());
            }

            // CREAR (POST)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(UserStatusViewModel vm)
            {
                if (!ModelState.IsValid) return View(vm);

                try
                {
                    var created = await _userStatusService.AddAsync(MapToDto(vm));
                    TempData["ok"] = $"Estado de usuario '{created?.Name}' creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(vm);
                }
            }
        

        // EDITAR (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sw = Stopwatch.StartNew();
            var dto = await _userStatusService.GetById(id);
            sw.Stop();
            Console.WriteLine($"GET Edit({id}) = {sw.ElapsedMilliseconds} ms");
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }



        // EDITAR (POST)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, RoleViewModel vm)
        //{
        //    if (id != vm.Id) return BadRequest();
        //    if (!ModelState.IsValid) return View(vm);

        //    try
        //    {
        //        var updated = await _roles.UpdateAsync(MapToDto(vm), id);
        //        if (updated is null) return NotFound();
        //        TempData["ok"] = $"Rol '{updated.Name}' actualizado.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        return View(vm);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserStatusViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var updated = await _userStatusService.UpdateAsync(MapToDto(vm), id);
            if (updated is null) return NotFound();

            TempData["ok"] = $"Estado de usuario '{updated.Name}' actualizado.";
            return RedirectToAction(nameof(Index));
        }


        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
            {
                var dto = await _userStatusService.GetById(id);
                if (dto is null) return NotFound();
                return View(MapToVm(dto));
            }

            // ELIMINAR (POST)
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _userStatusService.DeleteAsync(id);
                TempData["ok"] = "Estado de usuario eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // ---------- Helpers de mapeo (DTO <-> ViewModel) ----------
            private static UserStatusViewModel MapToVm(UserStatusDto dto) => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                // Extra opcional para mensajes/fechas en tu BaseViewModel:
                Message = null
            };

            private static UserStatusDto MapToDto(UserStatusViewModel vm) => new()
            {
                Id = vm.Id,
                Name = vm.Name
            };
        }
    }
