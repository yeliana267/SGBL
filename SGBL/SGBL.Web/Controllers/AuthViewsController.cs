using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Auth;
using SGBL.Application.Interfaces;
using System.Security.Claims;

namespace SGBL.Web.Controllers
{
    [AllowAnonymous] // Permitir acceso sin autenticación
    public class AuthViewsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthViewsController> _logger;

        public AuthViewsController(IAuthService authService, ILogger<AuthViewsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: /AuthViews/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir a su dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoleDashboard();
            }
            return View();
        }

        // POST: /AuthViews/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.LoginAsync(model);

                if (result.Success && result.ClaimsIdentity != null)
                {
                    // Iniciar sesión con cookies de autenticación
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(result.ClaimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = false, // Cambiar a true para "Recordarme"
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
                        });

                    _logger.LogInformation("✅ Usuario autenticado: {Email}", model.Email);

                    // Redirigir según el rol
                    return RedirectToRoleDashboard();
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Error en el login");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en login para {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(model);
            }
        }

        // GET: /AuthViews/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoleDashboard();
            }
            return View();
        }

        // POST: /AuthViews/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.RegisterAsync(model);

                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, result.Message ?? "Error en el registro");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en registro para {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(model);
            }
        }

        // GET: /AuthViews/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Token o email inválido.";
                    return RedirectToAction("Login");
                }

                var request = new ConfirmEmailRequestDto
                {
                    Token = token,
                    Email = email
                };

                var result = await _authService.ConfirmEmailAsync(request);

                if (result)
                {
                    TempData["Success"] = "¡Email confirmado exitosamente! Ya puedes iniciar sesión.";
                }
                else
                {
                    // ✅ MENSAJE MÁS ESPECÍFICO
                    TempData["Error"] = "El enlace de confirmación es inválido o ha expirado. Por favor solicita uno nuevo.";
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error confirmando email: {Email}", email);
                TempData["Error"] = "Error al confirmar el email.";
                return RedirectToAction("Login");
            }
        }

        // GET: /AuthViews/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /AuthViews/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.ForgotPasswordAsync(model);

                if (result)
                {
                    TempData["Success"] = "Si el email existe, recibirás instrucciones para restablecer tu contraseña.";
                }
                else
                {
                    TempData["Error"] = "Error al procesar la solicitud.";
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en forgot password para {Email}", model.Email);
                TempData["Error"] = "Error interno del servidor.";
                return RedirectToAction("Login");
            }
        }

        // GET: /AuthViews/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Token o email inválido.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordRequestDto
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        // POST: /AuthViews/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.ResetPasswordAsync(model);

                if (result)
                {
                    TempData["Success"] = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Token inválido o expirado.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en reset password para {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Error interno del servidor.");
                return View(model);
            }
        }

        // GET: /AuthViews/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("🚫 Acceso denegado para usuario: {User}", User.Identity.Name);
            return View();
        }

        // POST: /AuthViews/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("✅ Usuario cerró sesión: {User}", User.Identity.Name);
            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }

        // Método auxiliar para redirigir según el rol del usuario
        private IActionResult RedirectToRoleDashboard()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return role switch
            {
                "7" => RedirectToAction("Dashboard", "Admin"),        // ← ACTUALIZADO
                "9" => RedirectToAction("Dashboard", "UserDashboard"), // ← ACTUALIZADO
                "8" => RedirectToAction("Dashboard", "Bibliotecario"), // ← ACTUALIZADO
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}