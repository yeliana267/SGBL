
using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
namespace SGBL.Application.Services
{
    public class BookStatusService : GenericService<BookStatus,BookStatusDto>, IBookStatusService
    {
        private readonly IBookStatusRepository _bookStatusRepository;
        private readonly IMapper _mapper;
        public BookStatusService(IBookStatusRepository bookStatusRepository, IMapper mapper) : base(bookStatusRepository, mapper)
        {
            _bookStatusRepository = bookStatusRepository;
            _mapper = mapper;
        }

    }
}
