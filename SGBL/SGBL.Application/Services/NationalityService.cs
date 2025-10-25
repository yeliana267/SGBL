
using AutoMapper;
using SGBL.Application.Dtos.Nationality;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class NationalityService : GenericService<Nationality, NationalityDto>, INationalityService
    {
        private readonly INationalityRepository _nationalityRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;

        public NationalityService(INationalityRepository nationalityRepository, IMapper mapper, IServiceLogs serviceLogs) : base(nationalityRepository, mapper, serviceLogs)
        {
            _nationalityRepository = nationalityRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

    }
}
