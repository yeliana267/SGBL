using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class BookService : GenericService<Book, BookDto>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public BookService(IBookRepository bookRepository, IMapper mapper, IServiceLogs serviceLogs) : base(bookRepository, mapper,  serviceLogs)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    }
}
