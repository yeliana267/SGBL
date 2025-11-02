using SGBL.Application.Dtos.Auth;

namespace SGBL.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<bool> ConfirmEmailAsync(ConfirmEmailRequestDto request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<bool> IsEmailConfirmedAsync(string email);
    }
}