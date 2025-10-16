using SGBL.Application.Dtos.User;

namespace SGBL.Web.Controllers
{
    internal class CreateUserDto : UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
    }
}