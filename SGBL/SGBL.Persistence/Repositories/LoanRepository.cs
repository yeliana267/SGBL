using Microsoft.EntityFrameworkCore;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

public class LoanRepository : GenericRepository<Loan>, ILoanRepository
{
    private readonly SGBLContext _context;
    private readonly IServiceLogs _serviceLogs;

    public LoanRepository(SGBLContext context, IServiceLogs serviceLogs)
        : base(context, serviceLogs)
    {
        _context = context;
        _serviceLogs = serviceLogs;
    }

    public async Task<IEnumerable<Loan>> GetAllWithDetailsAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Include(l => l.LoanStatus)
            .OrderByDescending(l => l.CreationDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByUserAsync(int userId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Include(l => l.LoanStatus)
            .Where(l => l.IdUser == userId)
            .OrderByDescending(l => l.CreationDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Include(l => l.LoanStatus)
            .Where(l => l.Status == 2) // por ejemplo: solo “Recogido”
            .OrderByDescending(l => l.CreationDate)
            .ToListAsync();
    }


}
