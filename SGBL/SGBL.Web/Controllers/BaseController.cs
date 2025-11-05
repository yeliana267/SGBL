using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SGBL.Web.Controllers
{
    [Authorize] // Todos los controladores que hereden requieren autenticación
    public class BaseController : Controller
    {
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out int userId) ? userId : 0;
            }
        }

        protected string CurrentUserName
        {
            get { return User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario"; }
        }

        protected int CurrentUserRole
        {
            get
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                return int.TryParse(roleClaim, out int role) ? role : 9; // Por defecto User
            }
        }

        protected string CurrentUserEmail
        {
            get { return User.FindFirst(ClaimTypes.Email)?.Value ?? ""; }
        }

        protected string CurrentUserRoleName
        {
            get
            {
                return CurrentUserRole switch
                {
                    7 => "Administrador",     // ← ACTUALIZADO
                    9 => "Usuario",           // ← ACTUALIZADO  
                    8 => "Bibliotecario",     // ← ACTUALIZADO
                    _ => "Usuario"
                };
            }
        }


        // Método para verificar si el usuario tiene un rol específico
        protected bool HasRole(int roleId) => CurrentUserRole == roleId;

        // Método para verificar si el usuario tiene al menos uno de los roles especificados
        protected bool HasAnyRole(params int[] roleIds) => roleIds.Contains(CurrentUserRole);

        // Método para redirigir al dashboard según el rol
        protected IActionResult RedirectToRoleDashboard()
        {
            return CurrentUserRole switch
            {
                7 => RedirectToAction("Dashboard", "Admin"),     
                9 => RedirectToAction("Dashboard", "UserDashboard"), 
                8 => RedirectToAction("Dashboard", "Bibliotecario"), 
                _ => RedirectToAction("Login", "AuthViews")
            };
        }

        // Método para mostrar errores de forma consistente
        protected void AddModelError(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        // Método para loguear acciones
        protected void LogAction(string action)
        {
            // Aquí puedes integrar con tu sistema de logs
            Console.WriteLine($"[{DateTime.Now}] Usuario {CurrentUserName} ({CurrentUserEmail}) ejecutó: {action}");
        }
    }
}