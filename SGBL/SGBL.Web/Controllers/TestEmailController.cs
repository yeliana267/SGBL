using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Interfaces;

namespace SGBL.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<TestEmailController> _logger;

        public TestEmailController(IEmailService emailService, ILogger<TestEmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("quick-test")]
        public async Task<IActionResult> QuickTest([FromQuery] string email = "yelianadesarrollo@gmail.com")
        {
            try
            {
                _logger.LogInformation("📧 Intentando enviar correo a: {Email}", email);

                var testEmail = new EmailRequestDto
                {
                    To = email,
                    Subject = "🚀 Prueba Rápida - SocialNetwork",
                    HtmlBody = $@"
                        <html>
                        <body>
                            <h1>¡Prueba Exitosa! 🎉</h1>
                            <p>Hola <strong>Usuario de Prueba</strong>,</p>
                            <p>Este es un correo de prueba desde <strong>SGBL</strong>.</p>
                            <p>Si recibes este email, el servicio está funcionando correctamente.</p>
                            <p><em>Enviado el: {DateTime.Now}</em></p>
                        </body>
                        </html>"
                };

                await _emailService.SendAsync(testEmail);

                _logger.LogInformation("✅ Correo enviado exitosamente a: {Email}", email);

                return Ok(new
                {
                    success = true,
                    message = $"✅ Correo de prueba enviado a: {email}",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando correo a: {Email}", email);

                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new
            {
                status = "✅ TestEmailController está funcionando",
                timestamp = DateTime.Now,
                message = "El controlador está activo y respondiendo"
            });
        }
    }
}