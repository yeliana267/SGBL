
using AutoMapper;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Application.Dtos.Reminders;


namespace SGBL.Application.Services
{
    public class ReminderStatusService : GenericService<ReminderStatus, ReminderStatusDto>, IReminderStatusService
    {
        private readonly IReminderStatusRepository _IReminderStatusRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public ReminderStatusService(IReminderStatusRepository reminderStatusRepository, IMapper mapper, IServiceLogs serviceLogs) : base(reminderStatusRepository, mapper, serviceLogs)
        {
            _IReminderStatusRepository = reminderStatusRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    }
}
