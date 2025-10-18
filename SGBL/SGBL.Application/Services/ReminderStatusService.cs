
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
        public ReminderStatusService(IReminderStatusRepository reminderStatusRepository, IMapper mapper) : base(reminderStatusRepository, mapper)
        {
            _IReminderStatusRepository = reminderStatusRepository;
            _mapper = mapper;
        }
    }
}
