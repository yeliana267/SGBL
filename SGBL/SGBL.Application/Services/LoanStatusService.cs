using Application.Interfaces.Services;
using AutoMapper;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class LoanStatusService :GenericService<LoanStatus, LoanStatusDto>, ILoanStatusService
    {
        private readonly ILoanStatusRepository _loanStatusRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public LoanStatusService(ILoanStatusRepository loanStatusRepository, IMapper mapper, IServiceLogs serviceLogs) : base(loanStatusRepository, mapper, serviceLogs)
        {
            _loanStatusRepository = loanStatusRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    } 
}