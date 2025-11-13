
namespace SGBL.Application.ViewModels
{
    public class LoanViewModel : BaseViewModel<int>
    {
        public int IdBook { get; set; }
        public int IdUser { get; set; }
        public int IdLibrarian { get; set; }
        public DateTime DateLoan { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? PickupDeadline { get; set; }
        public int Status { get; set; }
        public decimal FineAmount { get; set; }
        public string Notes { get; set; }
    }
}
