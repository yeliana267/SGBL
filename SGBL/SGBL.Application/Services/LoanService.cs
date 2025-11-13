using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private const int LoanStatusPending = 1;
        private const int LoanStatusPickedUp = 2;
        private const int LoanStatusReturned = 3;
        private const int LoanStatusCancelled = 4;
        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMapper mapper, IServiceLogs serviceLogs) : base(loanRepository, mapper, serviceLogs)
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
        public async Task IncreaseAvailableCopies(int bookId)
        {
            var updated = await _bookRepository.AdjustAvailableCopiesAsync(bookId, 1);
            if (updated is null)
            {
                throw new KeyNotFoundException($"No se encontró el libro con id {bookId}.");
            }
        }


        public override async Task<LoanDto?> AddAsync(LoanDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var book = await _bookRepository.GetById(dto.IdBook);
            if (book is null)
            {
                throw new KeyNotFoundException($"No se encontró el libro con id {dto.IdBook}.");
            }

            if (book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException("El libro no tiene copias disponibles para préstamo.");
            }

            dto.Status = LoanStatusPending;
            dto.DateLoan = EnsureUtc(dto.DateLoan == default ? DateTime.UtcNow : dto.DateLoan);
            dto.DueDate = EnsureUtc(dto.DueDate == default ? dto.DateLoan.AddDays(7) : dto.DueDate);
            dto.PickupDeadline = EnsureUtc(dto.PickupDeadline ?? dto.DateLoan.AddDays(1));
            dto.PickupDate = EnsureUtc(dto.PickupDate);
            dto.ReturnDate = EnsureUtc(dto.ReturnDate);
            dto.CreatedAt = EnsureUtc(dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt);
            dto.UpdatedAt = EnsureUtc(dto.UpdatedAt ?? DateTime.UtcNow);

            await _bookRepository.AdjustAvailableCopiesAsync(dto.IdBook, -1);

            try
            {
                var created = await base.AddAsync(dto);
                return created;
            }
            catch
            {
                await _bookRepository.AdjustAvailableCopiesAsync(dto.IdBook, 1);
                throw;
            }
        }

        public override async Task<LoanDto?> UpdateAsync(LoanDto dto, int id)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var existing = await _loanRepository.GetById(id);
            if (existing is null)
            {
                return null;
            }

            var previousStatus = existing.Status;

            dto.DateLoan = EnsureUtc(dto.DateLoan);
            dto.DueDate = EnsureUtc(dto.DueDate);
            dto.PickupDeadline = EnsureUtc(dto.PickupDeadline);
            dto.PickupDate = EnsureUtc(dto.PickupDate);
            dto.ReturnDate = EnsureUtc(dto.ReturnDate);
            dto.CreatedAt = EnsureUtc(dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt);

            if (previousStatus != LoanStatusPickedUp && dto.Status == LoanStatusPickedUp && !dto.PickupDate.HasValue)
            {
                dto.PickupDate = DateTime.UtcNow;
            }

            var shouldRestoreStock = previousStatus != LoanStatusReturned && dto.Status == LoanStatusReturned;
            if (shouldRestoreStock && !dto.ReturnDate.HasValue)
            {
                dto.ReturnDate = DateTime.UtcNow;
            }

            dto.UpdatedAt = EnsureUtc(dto.UpdatedAt ?? DateTime.UtcNow);

            var updated = await base.UpdateAsync(dto, id);

            if (updated is not null && shouldRestoreStock)
            {
                await _bookRepository.AdjustAvailableCopiesAsync(dto.IdBook, 1);
            }

            return updated;
        }

        public async Task<int> CancelLoansNotPickedUpAsync()
        {
            var overdueLoans = await _loanRepository.GetPendingLoansPastPickupDeadlineAsync(DateTime.UtcNow, LoanStatusPending);

            if (!overdueLoans.Any())
            {
                return 0;
            }

            foreach (var loan in overdueLoans)
            {
                loan.Status = LoanStatusCancelled;
                loan.UpdatedAt = DateTime.UtcNow;
                loan.Notes = string.IsNullOrWhiteSpace(loan.Notes)
                    ? "Préstamo cancelado por no recoger a tiempo."
                    : loan.Notes + " | Préstamo cancelado por no recoger a tiempo.";

                await _bookRepository.AdjustAvailableCopiesAsync(loan.IdBook, 1);
            }

            return await _loanRepository.UpdateLoansAsync(overdueLoans);
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => value
            };
        }

        private static DateTime? EnsureUtc(DateTime? value)
        {
            return value.HasValue ? EnsureUtc(value.Value) : null;
        }




    }
}
