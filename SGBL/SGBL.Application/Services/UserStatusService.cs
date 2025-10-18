
using AutoMapper;
using SGBL.Application.Dtos.User;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class UserStatusService : GenericService<UserStatus,UserStatusDto>, IUserStatusService  
    {
        private readonly IUserStatusRepository _userStatusRepository;
        private readonly IMapper _mapper;
        public UserStatusService(IUserStatusRepository userStatusRepository, IMapper mapper) : base(userStatusRepository, mapper)
        {
            _userStatusRepository = userStatusRepository;
            _mapper = mapper;
        }
    }
}
