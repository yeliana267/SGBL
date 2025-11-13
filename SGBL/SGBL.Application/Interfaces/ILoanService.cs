using System.Collections.Generic;
using SGBL.Application.Dtos.Loan;

namespace SGBL.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task<List<LoanDto>> GetLoansDueInDays(int day);
        Task IncreaseAvailableCopies(int bookId);
        Task<int> CancelLoansNotPickedUpAsync();
    }

}
