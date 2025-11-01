using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.User
{
    public class UserDto: BaseAuditableDto<int>
    {
    public string Name { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public string? TokenActivation { get; set; }
        public string? TokenRecuperation { get; set; }
    }
}
