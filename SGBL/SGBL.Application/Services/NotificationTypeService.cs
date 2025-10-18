
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
        public NotificationTypeService(INotificationTypeRepository notificationTypeRepository, IMapper mapper) : base(notificationTypeRepository, mapper)
        {
            _INotificationTypeRepository = notificationTypeRepository;
            _mapper = mapper;
        }
    }
}
