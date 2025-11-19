using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

public interface ILoanRepository : IGenericRepository<Loan>
{
    Task<IEnumerable<Loan>> GetLoansByUserAsync(int userId);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();

    Task<IEnumerable<Loan>> GetAllWithDetailsAsync();

}
