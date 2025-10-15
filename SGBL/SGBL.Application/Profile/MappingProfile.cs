using AutoMapper;
using SGBL.Domain.Entities;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Dtos.Nationality;

namespace SGBL.Application.Profiles
{
    public class MappingProfile : Profile  // ya no hay conflicto
    {
        public MappingProfile()
        {
            CreateMap<Role, RoleDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
              

            CreateMap<RoleDto, Role>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CreatedAt, o => o.Ignore()) // no tocar en update
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Nationality, NationalityDto>()
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
            CreateMap<NationalityDto, Nationality>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CreatedAt, o => o.Ignore()) // no tocar en update
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));


        }
    }
}
