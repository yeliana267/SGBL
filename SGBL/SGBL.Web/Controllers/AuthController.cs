using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Auth;
using SGBL.Application.Interfaces;

namespace SGBL.Web.Controllers  
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                _logger.LogInformation("📝 Intento de registro para: {Email}", request.Email);
                var result = await _authService.RegisterAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Registro exitoso para: {Email}", request.Email);
                    return Ok(result);
                }

                _logger.LogWarning("❌ Registro fallido para: {Email} - {Message}", request.Email, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error en registro para: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔐 Intento de login para: {Email}", request.Email);
                var result = await _authService.LoginAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Login exitoso para: {Email}", request.Email);
                    return Ok(result);
                }

                _logger.LogWarning("❌ Login fallido para: {Email} - {Message}", request.Email, result.Message);
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error en login para: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
        {
            try
            {
                _logger.LogInformation("📧 Confirmando email: {Email}", request.Email);
                var result = await _authService.ConfirmEmailAsync(request);

                if (result)
                {
                    _logger.LogInformation("✅ Email confirmado: {Email}", request.Email);
                    return Ok(new { success = true, message = "Email confirmado exitosamente" });
                }

                _logger.LogWarning("❌ Confirmación fallida: {Email}", request.Email);
                return BadRequest(new { success = false, message = "Token inválido o expirado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error confirmando email: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔑 Solicitud de recuperación: {Email}", request.Email);
                var result = await _authService.ForgotPasswordAsync(request);

                if (result)
                {
                    _logger.LogInformation("✅ Email de recuperación enviado: {Email}", request.Email);
                    return Ok(new { success = true, message = "Si el email existe, recibirás instrucciones" });
                }

                _logger.LogWarning("❌ Error en recuperación: {Email}", request.Email);
                return BadRequest(new { success = false, message = "Error al procesar la solicitud" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error en forgot-password: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔄 Restableciendo contraseña: {Email}", request.Email);
                var result = await _authService.ResetPasswordAsync(request);

                if (result)
                {
                    _logger.LogInformation("✅ Contraseña restablecida: {Email}", request.Email);
                    return Ok(new { success = true, message = "Contraseña restablecida exitosamente" });
                }

                _logger.LogWarning("❌ Restablecimiento fallido: {Email}", request.Email);
                return BadRequest(new { success = false, message = "Token inválido o expirado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error en reset-password: {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmailConfirmed(string email)
        {
            try
            {
                _logger.LogInformation("📋 Verificando email: {Email}", email);
                var result = await _authService.IsEmailConfirmedAsync(email);
                return Ok(new { isConfirmed = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error verificando email: {Email}", email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        // Endpoint de diagnóstico
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new
            {
                status = "✅ AuthController está funcionando",
                timestamp = DateTime.Now,
                endpoints = new[] {
                    "POST /api/auth/register",
                    "POST /api/auth/login",
                    "POST /api/auth/confirm-email",
                    "POST /api/auth/forgot-password",
                    "POST /api/auth/reset-password",
                    "GET  /api/auth/check-email/{email}",
                    "GET  /api/auth/status"
                }
            });
        }
    }
}