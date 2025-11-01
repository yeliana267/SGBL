
namespace SGBL.Application.ViewModels
{
    public class GenreViewModel : BaseViewModel<int>
    {
        public override int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
