namespace SGBL.Application.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public string Name { get; set; }       // mapea con 'Name' de User
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }             // FK hacia roles
        public int Status { get; set; }           // FK hacia usuario_estados
    }
}
