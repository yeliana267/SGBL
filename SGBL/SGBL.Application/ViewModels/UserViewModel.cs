namespace SGBL.Application.ViewModels
{
    public class UserViewModel : BaseViewModel<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }

        public List<RoleViewModel> AvailableRoles { get; set; } = new List<RoleViewModel>();
        public List<UserStatusViewModel> AvailableStatuses { get; set; } = new List<UserStatusViewModel>();
    }
}