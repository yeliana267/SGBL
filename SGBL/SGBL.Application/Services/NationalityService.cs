
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
        public NationalityService(INationalityRepository nationalityRepository, IMapper mapper) : base(nationalityRepository, mapper)
        {
            _nationalityRepository = nationalityRepository;
            _mapper = mapper;
        }

    }
}
