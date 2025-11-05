using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    [Authorize(Roles = "7")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserStatusService _userStatusService;

        public UserController(IUserService userService, IRoleService roleService, IUserStatusService userStatusService)
        {
            _userService = userService;
            _roleService = roleService;
            _userStatusService = userStatusService;
        }

        // GET: /User/Index
        [HttpGet]
        public async Task<IActionResult> Index(string viewAction = "index", int? id = null)
        {
            var normalizedAction = (viewAction ?? "index").ToLower();
            ViewData["Action"] = normalizedAction;

            // Cargar roles y estados disponibles
            var roleDtos = await _roleService.GetAll();
            var statusDtos = await _userStatusService.GetAll();

            var availableRoles = roleDtos.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            var availableStatuses = statusDtos.Select(s => new UserStatusViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            switch (normalizedAction)
            {
                case "create":
                    var createVm = new UserViewModel
                    {
                        AvailableRoles = availableRoles,
                        AvailableStatuses = availableStatuses
                    };
                    return View(createVm);

                case "edit":
                case "delete":
                case "details":
                    if (!id.HasValue) return BadRequest();
                    var dto = await _userService.GetById(id.Value);
                    if (dto is null) return NotFound();

                    var vm = MapToVm(dto);
                    vm.AvailableRoles = availableRoles;
                    vm.AvailableStatuses = availableStatuses;
                    return View(vm);

                case "index":
                default:
                    var dtos = await _userService.GetAll();
                    var userList = dtos.Select(MapToVm).ToList();

                    // Asignar roles y estados a cada usuario en el listado
                    foreach (var user in userList)
                    {
                        user.AvailableRoles = availableRoles;
                        user.AvailableStatuses = availableStatuses;
                    }

                    ViewData["UserList"] = userList;
                    return View(new UserViewModel
                    {
                        AvailableRoles = availableRoles,
                        AvailableStatuses = availableStatuses
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string viewAction, UserViewModel vm)
        {
            var normalizedAction = (viewAction ?? "").ToLower();
            ViewData["Action"] = normalizedAction;

            // Cargar roles y estados en caso de error
            var roleDtos = await _roleService.GetAll();
            var statusDtos = await _userStatusService.GetAll();

            vm.AvailableRoles = roleDtos.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            vm.AvailableStatuses = statusDtos.Select(s => new UserStatusViewModel
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
                        var created = await _userService.AddAsync(MapToDto(vm));
                        TempData["success"] = $"Usuario '{created?.Name}' creado correctamente.";
                        break;

                    case "edit":
                        // Manejar password si está vacío
                        if (string.IsNullOrWhiteSpace(vm.Password))
                        {
                            var existingUser = await _userService.GetById(vm.Id);
                            if (existingUser != null)
                            {
                                vm.Password = existingUser.Password; // Mantener password actual
                            }
                        }

                        var updated = await _userService.UpdateAsync(MapToDto(vm), vm.Id);
                        TempData["success"] = $"Usuario '{updated?.Name}' actualizado correctamente.";
                        break;

                    case "delete":
                        await _userService.DeleteAsync(vm.Id);
                        TempData["success"] = "Usuario eliminado correctamente.";
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

        private static UserViewModel MapToVm(UserDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        private static UserDto MapToDto(UserViewModel vm) => new()
        {
            Id = vm.Id,
            Name = vm.Name,
            Email = vm.Email,
            Password = vm.Password,
            Role = vm.Role,
            Status = vm.Status
        };
    }
}