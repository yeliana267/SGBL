using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBL.Application.Services
{
    public class LoanService : GenericService<Loan, LoanDto>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IServiceLogs _serviceLogs;

        public LoanService(ILoanRepository loanRepository, IMapper mapper, IServiceLogs serviceLogs)
            : base(loanRepository, mapper, serviceLogs)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

        public async Task<List<LoanDto>> GetLoansDueInDays(int day)
        {
            if (day < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "El número de días debe ser mayor o igual a cero.");
            }

            try
            {
                var today = DateTime.UtcNow.Date;
                var limitExclusive = today.AddDays(day + 1);

                var query = _loanRepository.GetAllQuery();

                var loans = await query
                    .Where(loan => loan.ReturnDate == null &&
                                   loan.DueDate >= today &&
                                   loan.DueDate < limitExclusive)
                    .ToListAsync();

                return _mapper.Map<List<LoanDto>>(loans);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al obtener préstamos por vencer en {day} días: {ex}");
                throw;
            }
        }

        public async Task<bool> UserHasActiveLoanAsync(int userId, int bookId)
        {
            if (userId <= 0 || bookId <= 0)
            {
                return false;
            }

            try
            {
                var activeStatuses = new[] { 1, 2 };

                var query = _loanRepository
                    .GetAllQuery()
                    .AsNoTracking();

                return await query.AnyAsync(loan =>
                    loan.IdUser == userId &&
                    loan.IdBook == bookId &&
                    loan.ReturnDate == null &&
                    activeStatuses.Contains(loan.Status));
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error verificando préstamos activos del usuario {userId} para el libro {bookId}: {ex}");
                throw;
            }
        }
    }
}
