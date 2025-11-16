using System;
using System.Collections.Generic;
using SGBL.Domain.Entities;

namespace SGBL.Domain.Interfaces
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<List<Loan>> GetLoansDueInDaysAsync(int days);
        Task<List<Loan>> GetPendingLoansPastPickupDeadlineAsync(DateTime referenceDate, int pendingStatus);
        Task<int> UpdateLoansAsync(IEnumerable<Loan> loans);
    }
}
