
using AutoMapper;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Application.Dtos.Notification;



namespace SGBL.Application.Services
{
    public class NotificationTypeService : GenericService<NotificationType, NotificationTypeDto>, INotificationTypeService
    {
        private readonly INotificationTypeRepository _INotificationTypeRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public NotificationTypeService(INotificationTypeRepository notificationTypeRepository, IMapper mapper, IServiceLogs serviceLogs) : base(notificationTypeRepository, mapper, serviceLogs)
        {
            _INotificationTypeRepository = notificationTypeRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    }
}
