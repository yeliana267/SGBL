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

            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdBook, opt => opt.MapFrom(src => src.IdBook))
                .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.IdUser))
                .ForMember(dest => dest.IdLibrarian, opt => opt.MapFrom(src => src.IdLibrarian))
                .ForMember(dest => dest.DateLoan, opt => opt.MapFrom(src => src.DateLoan))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.PickupDate, opt => opt.MapFrom(src => src.PickupDate))
                .ForMember(dest => dest.PickupDeadline, opt => opt.MapFrom(src => src.PickupDeadline))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.FineAmount, opt => opt.MapFrom(src => src.FineAmount))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate))

                .ForMember(dest => dest.BookTitle,
                    opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : null))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Name : null))
                .ForMember(dest => dest.LibrarianName,
                    opt => opt.MapFrom(src => src.Librarian != null ? src.Librarian.Name : null))
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.LoanStatus != null ? src.LoanStatus.Name : null));



            // Reverse maps con configuración
            CreateMap<RoleDto, Role>().ApplyStandardReverseMapping();
            CreateMap<NationalityDto, Nationality>().ApplyStandardReverseMapping();
            CreateMap<UserStatusDto, UserStatus>().ApplyStandardReverseMapping();
            CreateMap<BookStatusDto, BookStatus>().ApplyStandardReverseMapping();
            CreateMap<ReminderStatusDto, ReminderStatus>().ApplyStandardReverseMapping();
            CreateMap<NotificationStatusDto, NotificationStatus>().ApplyStandardReverseMapping();
            CreateMap<NotificationTypeDto, NotificationType>().ApplyStandardReverseMapping();
            CreateMap<GenreDto, Genre>().ApplyStandardReverseMapping();
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

            CreateMap<LoanDto, Loan>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.IdBook, opt => opt.MapFrom(src => src.IdBook))
    .ForMember(dest => dest.IdUser, opt => opt.MapFrom(src => src.IdUser))
    .ForMember(dest => dest.IdLibrarian, opt => opt.MapFrom(src => src.IdLibrarian))
    .ForMember(dest => dest.DateLoan, opt => opt.MapFrom(src => src.DateLoan))
    .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
    .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
    .ForMember(dest => dest.PickupDate, opt => opt.MapFrom(src => src.PickupDate))
    .ForMember(dest => dest.PickupDeadline, opt => opt.MapFrom(src => src.PickupDeadline))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
    .ForMember(dest => dest.FineAmount, opt => opt.MapFrom(src => src.FineAmount))
    .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
    .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
    .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate));


        }



        private void ApplyCustomMappings()
        {
            CreateMap<User, UserDto>()
                .ApplyStandardMapping()
                .IgnoreSensitiveProperties();

            CreateMap<UserDto, User>()
                .ApplyStandardReverseMapping()
                .IgnoreSensitiveProperties();
            CreateMap<LoanDto, LoanViewModel>()
       .ReverseMap();
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