using AutoMapper;
using SGBL.Application.Dtos.Author;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Nationality;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Dtos.Reminders;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Dtos.User;
using SGBL.Application.ViewModels;
using SGBL.Domain.Entities;


namespace SGBL.Application.Profiles
{
    public class SmartMappingProfile : Profile
    {
        private static readonly string[] _sensitiveProperties = { "Password", "TokenActivation", "TokenRecuperation" };

        public SmartMappingProfile()
        {
            ApplyConventionMappings();
            ApplyCustomMappings();
        }

        private void ApplyConventionMappings()
        {
            // Mapeo automático para entidades con propiedades estándar
            CreateMap<Role, RoleDto>().ApplyStandardMapping();
            CreateMap<Nationality, NationalityDto>().ApplyStandardMapping();
            CreateMap<UserStatus, UserStatusDto>().ApplyStandardMapping();
            CreateMap<BookStatus, BookStatusDto>().ApplyStandardMapping();
            CreateMap<ReminderStatus, ReminderStatusDto>().ApplyStandardMapping();
            CreateMap<NotificationStatus, NotificationStatusDto>().ApplyStandardMapping();
            CreateMap<NotificationType, NotificationTypeDto>().ApplyStandardMapping();
            CreateMap<Genre, GenreDto>().ApplyStandardMapping();
            CreateMap<Loan, LoanDto>().ApplyStandardMapping();
            CreateMap<LoanStatus, LoanStatusDto>().ApplyStandardMapping();
            CreateMap<Book, BookDto>().ApplyStandardMapping();
            CreateMap<Author, AuthorDto>().ApplyStandardMapping();
           

       
            


            CreateMap<Book, BookDto>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Isbn, o => o.MapFrom(s => s.Isbn))   
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.PublicationYear, o => o.MapFrom(s => s.PublicationYear))
                .ForMember(d => d.Pages, o => o.MapFrom(s => s.Pages))
                .ForMember(d => d.TotalCopies, o => o.MapFrom(s => s.TotalCopies))
                .ForMember(d => d.AvailableCopies, o => o.MapFrom(s => s.AvailableCopies))
                .ForMember(d => d.Ubication, o => o.MapFrom(s => s.Ubication))
                .ForMember(d => d.StatusId, o => o.MapFrom(s => s.Status));

            // Reverse maps con configuración
            CreateMap<RoleDto, Role>().ApplyStandardReverseMapping();
            CreateMap<NationalityDto, Nationality>().ApplyStandardReverseMapping();
            CreateMap<UserStatusDto, UserStatus>().ApplyStandardReverseMapping();
            CreateMap<BookStatusDto, BookStatus>().ApplyStandardReverseMapping();
            CreateMap<ReminderStatusDto, ReminderStatus>().ApplyStandardReverseMapping();
            CreateMap<NotificationStatusDto, NotificationStatus>().ApplyStandardReverseMapping();
            CreateMap<NotificationTypeDto, NotificationType>().ApplyStandardReverseMapping();
            CreateMap<GenreDto, Genre>().ApplyStandardReverseMapping();
            CreateMap<LoanDto, Loan>().ApplyStandardReverseMapping();
            CreateMap<LoanStatusDto, LoanStatus>().ApplyStandardReverseMapping();
            CreateMap<BookDto, Book>().ApplyStandardReverseMapping();
            CreateMap<AuthorDto, Author>().ApplyStandardReverseMapping();
           


           



            CreateMap<BookDto, Book>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Isbn, o => o.MapFrom(s => s.Isbn))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.PublicationYear, o => o.MapFrom(s => s.PublicationYear))
                .ForMember(d => d.Pages, o => o.MapFrom(s => s.Pages))
                .ForMember(d => d.TotalCopies, o => o.MapFrom(s => s.TotalCopies))
                .ForMember(d => d.AvailableCopies, o => o.MapFrom(s => s.AvailableCopies))
                .ForMember(d => d.Ubication, o => o.MapFrom(s => s.Ubication))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.StatusId)) 
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

        }


        private void ApplyCustomMappings()
        {
            CreateMap<User, UserDto>()
                .ApplyStandardMapping()
                .IgnoreSensitiveProperties();

            CreateMap<UserDto, User>()
                .ApplyStandardReverseMapping()
                .IgnoreSensitiveProperties();
        }
    }

    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> ApplyStandardMapping<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceProps = typeof(TSource).GetProperties();
            var destinationProps = typeof(TDestination).GetProperties();

            foreach (var destProp in destinationProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);
                if (sourceProp != null)
                {
                    expression.ForMember(destProp.Name, opt => opt.MapFrom(sourceProp.Name));
                }
            }

            return expression;
        }

        public static IMappingExpression<TSource, TDestination> ApplyStandardReverseMapping<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            return expression
                .ApplyStandardMapping()
                .ForMember("CreatedAt", opt => opt.Ignore())
                .ForMember("UpdatedAt", opt => opt.MapFrom(_ => DateTime.UtcNow));
        }

        public static IMappingExpression<TSource, TDestination> IgnoreSensitiveProperties<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var sensitiveProps = new[] { "Password", "TokenActivation", "TokenRecuperation" };

            foreach (var prop in sensitiveProps)
            {
                var destProp = typeof(TDestination).GetProperty(prop);
                if (destProp != null)
                {
                    expression.ForMember(prop, opt => opt.Ignore());
                }
            }

            return expression;
        }
    }
}