using SGBL.Application.Dtos.Loan;

namespace SGBL.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task<bool> ReturnBookAsync(int loanId, int librarianId);
        Task<IEnumerable<LoanDto>> GetLoansByUserAsync(int userId);
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<bool> MarkAsPickedUpAsync(int loanId, int librarianId);
    }
}