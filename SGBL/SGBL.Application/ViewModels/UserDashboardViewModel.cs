using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBL.Application.ViewModels
{
    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;

        // estadísticas
        public int ActiveLoansCount { get; set; }
        public int ReadBooksCount { get; set; }

        // próxima devolución
        public string? NextDueBookTitle { get; set; }
        public int? NextDueInDays { get; set; }

        // lista de préstamos activos resumidos
        public List<UserLoanSummaryViewModel> ActiveLoans { get; set; } = new();
    }
    public class UserLoanSummaryViewModel
    {
        public int LoanId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? AuthorName { get; set; }  // opcional si luego lo llenas
        public DateTime? DateLoan { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysRemaining { get; set; }
    }
}