using AutoMapper;
using SGBL.Domain.Entities;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Dtos.Nationality;
using SGBL.Application.Dtos.User;
using SGBL.Application.Services;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Dtos.Reminders;

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


            CreateMap<UserStatus, UserStatusDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<UserStatusDto, UserStatus>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>()
    .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
    .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
    .ForMember(d => d.Role, o => o.MapFrom(s => s.Role))
    .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
    .ForMember(d => d.Password, o => o.Ignore())           // 🔒 no los mandes al cliente
    .ForMember(d => d.TokenActivation, o => o.Ignore())
    .ForMember(d => d.TokenRecuperation, o => o.Ignore());

            // UserDto -> User (no pises secretos ni fechas)
            CreateMap<UserDto, User>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
    .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
        .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Password, o => o.Ignore())           // manejar en service
                .ForMember(d => d.TokenActivation, o => o.Ignore())
                .ForMember(d => d.TokenRecuperation, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<BookStatus, BookStatusDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<BookStatusDto, BookStatus>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ReminderStatus, ReminderStatusDto>()
           .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
           .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

            CreateMap<ReminderStatusDto, ReminderStatus>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));



        }
    }
}
