

using SGBL.Application.Dtos.Email;

namespace SGBL.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}
