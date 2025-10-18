using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SGBL.Application.Dtos.Reminders;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Application.ViewModels;
using System.Diagnostics;

namespace SGBL.Web.Controllers
{
    public class ReminderStatusController : Controller
    {
            private readonly IReminderStatusService _reminderStatus;

            public ReminderStatusController(IReminderStatusService reminderStatus)
            {
                _reminderStatus = reminderStatus;
            }

        // LISTADO
        [HttpGet]
        [OutputCache(Duration = 30)]
        public async Task<IActionResult> Index()
        {
            var dtos = await _reminderStatus.GetAll(); // ya AsNoTracking en repo
            var vms = dtos.Select(MapToVm).ToList();
            return View(vms);
        }


        // DETALLE
        public async Task<IActionResult> Details(int id)
            {
                var dto = await _reminderStatus.GetById(id);
                if (dto is null) return NotFound();
                return View(MapToVm(dto));
            }

            // CREAR (GET)
            public IActionResult Create()
            {
                return View(new ReminderStatusViewModel());
            }

            // CREAR (POST)
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(ReminderStatusViewModel vm)
            {
                if (!ModelState.IsValid) return View(vm);

                try
                {
                    var created = await _reminderStatus.AddAsync(MapToDto(vm));
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
            var dto = await _reminderStatus.GetById(id);
            sw.Stop();
            Console.WriteLine($"GET Edit({id}) = {sw.ElapsedMilliseconds} ms");
            if (dto is null) return NotFound();
            return View(MapToVm(dto));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReminderStatusViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var updated = await _reminderStatus.UpdateAsync(MapToDto(vm), id);
            if (updated is null) return NotFound();

            TempData["ok"] = $"Recordatiorio estado '{updated.Name}' actualizado.";
            return RedirectToAction(nameof(Index));
        }


        // ELIMINAR (GET)
        public async Task<IActionResult> Delete(int id)
            {
                var dto = await _reminderStatus.GetById(id);
                if (dto is null) return NotFound();
                return View(MapToVm(dto));
            }

            // ELIMINAR (POST)
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _reminderStatus.DeleteAsync(id);
                TempData["ok"] = "Estado del recordatorio eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // ---------- Helpers de mapeo (DTO <-> ViewModel) ----------
            private static ReminderStatusViewModel MapToVm(ReminderStatusDto dto) => new()
            {
                Id = dto.Id,
                Name = dto.Name,
                // Extra opcional para mensajes/fechas en tu BaseViewModel:
                Message = null
            };

            private static ReminderStatusDto MapToDto(ReminderStatusViewModel vm) => new()
            {
                Id = vm.Id,
                Name = vm.Name
            };
        }
    }
