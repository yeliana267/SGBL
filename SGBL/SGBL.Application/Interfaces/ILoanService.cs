
using SGBL.Application.Dtos.Loan;

namespace SGBL.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task GetLoansDueInDays(int day);
    }
}
