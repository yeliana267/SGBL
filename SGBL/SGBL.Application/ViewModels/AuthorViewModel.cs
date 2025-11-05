
namespace SGBL.Application.ViewModels
{
    public class AuthorViewModel : BaseViewModel<int>
    {

        public string Name { get; set; }
        public string Biography { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Nationality { get; set; }
        public List<NationalityViewModel>? Nationalities { get; set; }
    }
}
