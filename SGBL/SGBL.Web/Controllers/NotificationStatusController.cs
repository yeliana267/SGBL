using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Application.ViewModels;
using System.Diagnostics;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]
    public class NotificationStatusController : Controller
    {
            private readonly INotificationStatusService _NotificationStatus;

            public NotificationStatusController(INotificationStatusService NotificationStatus)
            {
                _NotificationStatus = NotificationStatus;
            }

        // LISTADO
        [HttpGet]
        [OutputCache(Duration = 30)]
        public async Task<IActionResult> Index()
        {
            var dtos = await _NotificationStatus.GetAll(); // ya AsNoTracking en repo
            var vms = dtos.Select(MapToVm).ToList();
            return View(vms);
        }


        // DETALLE
        public async Task<IActionResult> Details(int id)
            {
                var dto = await _NotificationStatus.GetById(id);
                if (dto is null) return NotFound();
                return View(MapToVm(dto));
            }

            // CREAR (GET)
            public IActionResult Create()
            {
                return View(new NotificationStatusViewModel());
            }

            // CREAR (POST)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(NotificationStatusViewModel vm)
            {
                if (!ModelState.IsValid) return View(vm);

                try
                {
                    var created = await _NotificationStatus.AddAsync(MapToDto(vm));
                    TempData["ok"] = $"Recordatorio estado '{created?.Name}' creado correctamente.";
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
            var dto = await _NotificationStatus.GetById(id);
            sw.Stop();
            Console.WriteLine($"GET Edit({id}) = {sw.ElapsedMilliseconds} ms");
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NotificationStatusViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var updated = await _NotificationStatus.UpdateAsync(MapToDto(vm), id);
            if (updated is null) return NotFound();

            TempData["ok"] = $"Recordatiorio estado '{updated.Name}' actualizado.";
            return RedirectToAction(nameof(Index));
        }


        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
            {
                var dto = await _NotificationStatus.GetById(id);
                if (dto is null) return NotFound();
                return View(MapToVm(dto));
            }

            // ELIMINAR (POST)
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _NotificationStatus.DeleteAsync(id);
                TempData["ok"] = "Estado del recordatorio eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // ---------- Helpers de mapeo (DTO <-> ViewModel) ----------
            private static NotificationStatusViewModel MapToVm(NotificationStatusDto dto) => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                // Extra opcional para mensajes/fechas en tu BaseViewModel:
                Message = null
            };

            private static NotificationStatusDto MapToDto(NotificationStatusViewModel vm) => new()
            {
                Id = vm.Id,
                Name = vm.Name
            };
        }
    }
