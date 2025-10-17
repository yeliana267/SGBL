
using AutoMapper;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Application.Dtos.Notification;



namespace SGBL.Application.Services
{
    public class NotificationStatusService : GenericService<NotificationStatus, NotificationStatusDto>, INotificationStatusService
    {
        private readonly INotificationStatusRepository _INotificationtatusRepository;
        private readonly IMapper _mapper;
        public NotificationStatusService(INotificationStatusRepository notificationStatusRepository, IMapper mapper) : base(notificationStatusRepository, mapper)
        {
            _INotificationtatusRepository = notificationStatusRepository;
            _mapper = mapper;
        }
    }
}
