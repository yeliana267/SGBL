using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        private readonly SGBLContext _context;
        private readonly IServiceLogs _serviceLogs;
        public LoanRepository(SGBLContext context, IServiceLogs serviceLogs) : base(context, serviceLogs)
        {
            _context = context;
            _serviceLogs = serviceLogs;
        }
        public async Task<List<Loan>> GetLoansDueInDaysAsync(int days)
        {
            var today = DateTime.UtcNow.Date;
            var limit = today.AddDays(days + 1);

            return await _context.Loans
                .AsNoTracking()
                .Where(loan => loan.ReturnDate == null
                               && loan.DueDate >= today
                               && loan.DueDate < limit)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetPendingLoansPastPickupDeadlineAsync(DateTime referenceDate, int pendingStatus)
        {
            var utcReference = referenceDate.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(referenceDate, DateTimeKind.Utc),
                DateTimeKind.Local => referenceDate.ToUniversalTime(),
                _ => referenceDate
            };

            return await _context.Loans
                .Where(loan => loan.Status == pendingStatus
                               && loan.PickupDeadline < utcReference
                               && loan.PickupDate == null
                               && loan.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<int> UpdateLoansAsync(IEnumerable<Loan> loans)
        {
            _context.Loans.UpdateRange(loans);
            return await _context.SaveChangesAsync();
        }
    }
}
