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

        public BookService(IBookRepository bookRepository, IMapper mapper) : base(bookRepository, mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
    }
}
