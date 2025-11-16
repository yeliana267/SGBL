using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using Assert = Xunit.Assert;

namespace SGBL.Application.Tests.Services
{
    public class LoanServiceTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IServiceLogs> _serviceLogsMock;
        private readonly LoanService _loanService;

        public LoanServiceTests()
        {
            _loanRepositoryMock = new Mock<ILoanRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _serviceLogsMock = new Mock<IServiceLogs>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Loan, LoanDto>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _loanService = new LoanService(
                _loanRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _mapper,
                _serviceLogsMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_CreateLoan_AndDecreaseBookStock()
        {
            var now = DateTime.UtcNow;

            var dto = new LoanDto
            {
                IdBook = 1,
                IdUser = 2,
                IdLibrarian = 3,
                PickupDeadline = now.AddDays(1),
                DueDate = now.AddDays(7)
            };

            var book = new Book { Id = 1, AvailableCopies = 2, TotalCopies = 5 };

            _bookRepositoryMock.Setup(r => r.GetById(dto.IdBook)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.AdjustAvailableCopiesAsync(dto.IdBook, -1))
                .ReturnsAsync(new Book { Id = 1, AvailableCopies = 1, TotalCopies = 5 });

            _loanRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Loan>()))
                .ReturnsAsync((Loan loan) =>
                {
                    loan.Id = 10;
                    return loan;
                });

            var result = await _loanService.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal(1, result.Status.GetValueOrDefault());

            _bookRepositoryMock.Verify(r => r.AdjustAvailableCopiesAsync(dto.IdBook, -1), Times.Once);
            _loanRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ToPickedUp_Should_SetPickupDate()
        {
            var now = DateTime.UtcNow;

            var existingLoan = new Loan
            {
                Id = 20,
                IdBook = 5,
                Status = 1,
                PickupDeadline = now.AddHours(-1),
                DueDate = now.AddDays(5),
                PickupDate = default,
                ReturnDate = default,
                CreatedAt = now.AddDays(-3)
            };

            _loanRepositoryMock.Setup(r => r.GetById(existingLoan.Id)).ReturnsAsync(existingLoan);
            _loanRepositoryMock.Setup(r => r.UpdateAsync(existingLoan.Id, It.IsAny<Loan>()))
                .ReturnsAsync((int id, Loan loan) => loan);

            var dto = new LoanDto
            {
                Id = existingLoan.Id,
                IdBook = existingLoan.IdBook,
                Status = 2
            };

            var result = await _loanService.UpdateAsync(dto, dto.Id);

            Assert.NotNull(result);
            Assert.Equal(2, result!.Status.GetValueOrDefault());

            _loanRepositoryMock.Verify(r => r.UpdateAsync(dto.Id, It.Is<Loan>(l => l.PickupDate != default)), Times.Once);
            _bookRepositoryMock.Verify(r => r.AdjustAvailableCopiesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ToReturned_Should_IncreaseBookStock()
        {
            var now = DateTime.UtcNow;

            var existingLoan = new Loan
            {
                Id = 30,
                IdBook = 7,
                Status = 2,
                PickupDeadline = now.AddDays(-2),
                DueDate = now.AddDays(3),
                PickupDate = now.AddDays(-1),
                ReturnDate = default,
                CreatedAt = now.AddDays(-6)
            };

            _loanRepositoryMock.Setup(r => r.GetById(existingLoan.Id)).ReturnsAsync(existingLoan);
            _loanRepositoryMock.Setup(r => r.UpdateAsync(existingLoan.Id, It.IsAny<Loan>()))
                .ReturnsAsync((int id, Loan loan) => loan);

            _bookRepositoryMock.Setup(r => r.AdjustAvailableCopiesAsync(existingLoan.IdBook, 1))
                .ReturnsAsync(new Book { Id = existingLoan.IdBook, AvailableCopies = 4, TotalCopies = 5 });

            var dto = new LoanDto
            {
                Id = existingLoan.Id,
                IdBook = existingLoan.IdBook,
                Status = 3
            };

            var result = await _loanService.UpdateAsync(dto, dto.Id);

            Assert.NotNull(result);
            Assert.Equal(3, result!.Status.GetValueOrDefault());

            _bookRepositoryMock.Verify(r => r.AdjustAvailableCopiesAsync(existingLoan.IdBook, 1), Times.Once);
        }

        [Fact]
        public async Task CancelLoansNotPickedUpAsync_Should_CancelAndRestoreStock()
        {
            var overdueLoans = new List<Loan>
            {
                new Loan
                {
                    Id = 40,
                    IdBook = 11,
                    Status = 1,
                    PickupDeadline = DateTime.UtcNow.AddDays(-2),
                    PickupDate = default,
                    ReturnDate = default,
                    Notes = string.Empty,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };

            _loanRepositoryMock.Setup(r => r.GetPendingLoansPastPickupDeadlineAsync(It.IsAny<DateTime>(), 1))
                .ReturnsAsync(overdueLoans);

            _bookRepositoryMock.Setup(r => r.AdjustAvailableCopiesAsync(11, 1))
                .ReturnsAsync(new Book { Id = 11, AvailableCopies = 6, TotalCopies = 6 });

            _loanRepositoryMock.Setup(r => r.UpdateLoansAsync(overdueLoans)).ReturnsAsync(overdueLoans.Count);

            var cancelledCount = await _loanService.CancelLoansNotPickedUpAsync();

            Assert.Equal(1, cancelledCount);
            Assert.Equal(4, overdueLoans[0].Status.GetValueOrDefault());

            _bookRepositoryMock.Verify(r => r.AdjustAvailableCopiesAsync(11, 1), Times.Once);
            _loanRepositoryMock.Verify(r => r.UpdateLoansAsync(overdueLoans), Times.Once);
        }
    }
}