using AutoMapper;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class RoleService : GenericService<Role, RoleDto>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public RoleService(IRoleRepository roleRepository, IMapper mapper, IServiceLogs serviceLogs) : base(roleRepository, mapper, serviceLogs)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    }
}
