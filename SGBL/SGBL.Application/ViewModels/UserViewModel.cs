namespace SGBL.Application.ViewModels
{
    public class UserViewModel : BaseViewModel<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }      
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }           
        public int Status { get; set; }          
    }
}
