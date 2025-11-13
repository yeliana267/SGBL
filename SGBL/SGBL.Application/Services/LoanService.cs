
using System.Threading.Tasks;
using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class LoanService : GenericService<Loan, LoanDto>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public LoanService(ILoanRepository loanRepository, IMapper mapper, IServiceLogs serviceLogs) : base(loanRepository, mapper, serviceLogs)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

        public async Task GetLoansDueInDays(int day)
        {

        }

        public async Task IncreaseAvailableCopies(int bookId)
        {

        }





    }
}
