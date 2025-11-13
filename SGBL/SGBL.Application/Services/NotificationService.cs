
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SGBL.Application.Dtos.Auth;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class NotificationService : GenericService<Notification, NotificationDto>, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        private readonly IEmailService _emailService;


        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IServiceLogs serviceLogs, IEmailService emailService) : base(notificationRepository, mapper, serviceLogs)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
            _emailService = emailService;
        }



       

    }
}
