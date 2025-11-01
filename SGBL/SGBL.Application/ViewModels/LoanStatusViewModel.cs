
namespace SGBL.Application.ViewModels
{
    public class LoanStatusViewModel : BaseViewModel<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
