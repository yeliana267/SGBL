using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBL.Application.Services
{
    public class BookService : GenericService<Book, BookDto>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;

        public BookService(IBookRepository bookRepository, IMapper mapper, IServiceLogs serviceLogs)
            : base(bookRepository, mapper, serviceLogs)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

        public async Task<BookDto> AddAsync(BookDto dto)
        {
            // ✅ VALIDAR ISBN DUPLICADO - CORREGIDO: Verificar null
            var existingBooks = await _bookRepository.GetAllAsync();
            if (existingBooks != null && existingBooks.Any(b => b.Isbn == dto.Isbn))
            {
                throw new System.InvalidOperationException("ISBN ya registrado");
            }

            // ✅ VALIDAR STOCK NEGATIVO
            if (dto.AvailableCopies < 0 || dto.TotalCopies < 0)
            {
                throw new System.ArgumentException("El stock no puede ser negativo");
            }

            // ✅ VALIDAR TÍTULO VACÍO
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new System.ArgumentException("El título es requerido");
            }

            // ✅ VALIDAR AÑO DE PUBLICACIÓN
            var currentYear = System.DateTime.Now.Year;
            if (dto.PublicationYear < 1000 || dto.PublicationYear > currentYear + 1)
            {
                throw new System.ArgumentException($"El año de publicación debe estar entre 1000 y {currentYear + 1}");
            }

            var entity = _mapper.Map<Book>(dto);
            await _bookRepository.AddAsync(entity);
            return _mapper.Map<BookDto>(entity);
        }
    }
}