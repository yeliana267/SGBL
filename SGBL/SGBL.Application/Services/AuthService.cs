using AutoMapper;
using Microsoft.Extensions.Configuration;
using SGBL.Application.Dtos.Auth;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SGBL.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;

        public AuthService(
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration,
            IMapper mapper,
            IServiceLogs serviceLogs)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Registro iniciado para: {request.Email}");

                var existingUser = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == request.Email);

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "El email ya está registrado"
                    };
                }

                // Crear nuevo usuario PENDIENTE de confirmación
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = HashPassword(request.Password),
                    Role = 9, // User = 9
                    Status = 6, // PENDIENTE = 6
                    TokenActivation = GenerateToken(),
                    TokenRecuperation = string.Empty,
                    TokenActivationExpires = DateTime.UtcNow.AddHours(24), // ✅ 24 horas de validez
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await SendConfirmationEmail(user);

                _serviceLogs.CreateLogInfo($"Usuario registrado: {request.Email} con token válido por 24 horas");

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Usuario registrado. Revisa tu email para activar la cuenta."
                };
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en registro: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Error al registrar el usuario"
                };
            }
        }

        public async Task<bool> ConfirmEmailAsync(ConfirmEmailRequestDto request)
        {
            try
            {
                var user = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == request.Email && u.TokenActivation == request.Token);

                if (user == null)
                {
                    _serviceLogs.CreateLogWarning($"Token no encontrado para: {request.Email}");
                    return false;
                }

                // ✅ VERIFICAR SI EL TOKEN EXPIRÓ
                if (user.TokenActivationExpires < DateTime.UtcNow)
                {
                    _serviceLogs.CreateLogWarning($"Token expirado para: {request.Email}");
                    return false;
                }

                // ✅ VERIFICAR SI EL USUARIO YA ESTÁ ACTIVO
                if (user.Status == 5) // Ya está activo
                {
                    _serviceLogs.CreateLogInfo($"Usuario ya estaba activo: {request.Email}");
                    return true;
                }

                user.Status = 5; // Activar cuenta
                user.TokenActivation = string.Empty;
                user.TokenActivationExpires = null; // Limpiar expiración
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user.Id, user);

                _serviceLogs.CreateLogInfo($"Email confirmado: {request.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error confirmando email: {ex.Message}");
                return false;
            }
        }


      
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Login intentado para: {request.Email}");

                var user = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == request.Email);

                if (user == null || !VerifyPassword(request.Password, user.Password))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email o contraseña incorrectos"
                    };
                }

                // ✅ VERIFICAR SI EL USUARIO ESTÁ PENDIENTE (no confirmado)
                if (user.Status == 6) // Pendiente = no confirmado
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Por favor confirma tu email antes de iniciar sesión. Revisa tu bandeja de entrada."
                    };
                }

                // Estado activo es 5
                if (user.Status != 5) // Cuenta suspendida u otro estado
                {
                    if (user.Status == 4) // Suspendido
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "Tu cuenta está suspendida. Contacta al administrador."
                        };
                    }
                    else
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "Tu cuenta no está activa. Contacta al administrador."
                        };
                    }
                }

                // Crear claims identity para la cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                var userDto = new UserAuthDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };

                _serviceLogs.CreateLogInfo($"Login exitoso: {request.Email}");

                return new AuthResponseDto
                {
                    Success = true,
                    User = userDto,
                    Message = "Login exitoso",
                    ClaimsIdentity = claimsIdentity
                };
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en login: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Error al iniciar sesión"
                };
            }
        }
        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            try
            {
                var user = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == email);

                if (user == null) return true; // Por seguridad

                // Renovar token
                user.TokenActivation = GenerateToken();
                user.TokenActivationExpires = DateTime.UtcNow.AddHours(24);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user.Id, user);

                // Reenviar email
                await SendConfirmationEmail(user);

                _serviceLogs.CreateLogInfo($"Email de confirmación reenviado: {email}");

                return true;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error reenviando confirmación: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                var user = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == request.Email);

                if (user == null) return true; // Por seguridad, no revelar si el email existe

                user.TokenRecuperation = GenerateToken();
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user.Id, user);

                // Enviar email para recuperar contraseña
                await SendPasswordResetEmail(user);

                _serviceLogs.CreateLogInfo($"Solicitud de recuperación enviada: {request.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en recuperación: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                var user = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == request.Email && u.TokenRecuperation == request.Token);

                if (user == null) return false;

                user.Password = HashPassword(request.NewPassword);
                user.TokenRecuperation = string.Empty;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user.Id, user);

                _serviceLogs.CreateLogInfo($"Contraseña restablecida: {request.Email}");

                return true;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error restableciendo contraseña: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsEmailConfirmedAsync(string email)
        {
            var user = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Email == email);

            return user?.Status == 5; // ✅ Activo = 5
        }

        // Métodos auxiliares privados
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        private async Task SendConfirmationEmail(User user)
        {
            var confirmationLink = $"{_configuration["App:BaseUrl"]}/AuthViews/ConfirmEmail?token={user.TokenActivation}&email={user.Email}";

            var emailRequest = new EmailRequestDto
            {
                To = user.Email,
                Subject = "Confirma tu cuenta - SGBL Sistema",
                HtmlBody = EmailTemplateService.CreateEmailConfirmationTemplate(
                    user.Name,
                    confirmationLink)
            };

            await _emailService.SendAsync(emailRequest);
        }

        private async Task SendPasswordResetEmail(User user)
        {
            var resetLink = $"{_configuration["App:BaseUrl"]}/AuthViews/ResetPassword?token={user.TokenRecuperation}&email={user.Email}";

            var emailRequest = new EmailRequestDto
            {
                To = user.Email,
                Subject = "Restablecer contraseña - SGBL Sistema",
                HtmlBody = EmailTemplateService.CreatePasswordResetTemplate(
                    user.Name,
                    resetLink)
            };

            await _emailService.SendAsync(emailRequest);
        }
    }
}