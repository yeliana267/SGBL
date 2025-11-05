using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SGBL.Application.Dtos.Nationality;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using System.Diagnostics;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]
    public class NationalityController : Controller
{
private readonly INationalityService _nationality;

public NationalityController(INationalityService nationality)
{
    _nationality = nationality;
}

// LISTADO
[HttpGet]
[OutputCache(Duration = 30)]
public async Task<IActionResult> Index()
{
    var dtos = await _nationality.GetAll(); // ya AsNoTracking en repo
    var vms = dtos.Select(MapToVm).ToList();
    return View(vms);
}


// DETALLE
public async Task<IActionResult> Details(int id)
{
    var dto = await _nationality.GetById(id);
    if (dto is null) return NotFound();
    return View(MapToVm(dto));
}

// CREAR (GET)
public IActionResult Create()
{
    return View(new NationalityViewModel());
}

// CREAR (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(NationalityViewModel vm)
{
    if (!ModelState.IsValid) return View(vm);

    try
    {
        var created = await _nationality.AddAsync(MapToDto(vm));
        TempData["ok"] = $"Nacionalidad'{created?.Name}' creada correctamente.";
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
    var dto = await _nationality.GetById(id);
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
//        var updated = await _nationality.UpdateAsync(MapToDto(vm), id);
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
public async Task<IActionResult> Edit(int id, NationalityViewModel vm)
{
    if (id != vm.Id) return BadRequest();
    if (!ModelState.IsValid) return View(vm);

    var updated = await _nationality.UpdateAsync(MapToDto(vm), id);
    if (updated is null) return NotFound();

    TempData["ok"] = $"Nacionalidad '{updated.Name}' actualizada.";
    return RedirectToAction(nameof(Index));
}


// ELIMINAR (GET)
public async Task<IActionResult> Delete(int id)
{
    var dto = await _nationality.GetById(id);
    if (dto is null) return NotFound();
    return View(MapToVm(dto));
}

// ELIMINAR (POST)
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    await _nationality.DeleteAsync(id);
    TempData["ok"] = "Nacionalidad eliminada.";
    return RedirectToAction(nameof(Index));
}

// ---------- Helpers de mapeo (DTO <-> ViewModel) ----------
private static NationalityViewModel MapToVm(NationalityDto dto) => new()
{
    Id = dto.Id,
    Name = dto.Name,
    // Extra opcional para mensajes/fechas en tu BaseViewModel:
    Message = null
};

private static NationalityDto MapToDto(NationalityViewModel vm) => new()
{
    Id = vm.Id,
    Name = vm.Name
};
}
}

