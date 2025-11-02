using AutoMapper;
using SGBL.Application.Dtos.Author;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class AuthorService : GenericService<Author,AuthorDto>, IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        
        public AuthorService(IAuthorRepository authorRepository, IMapper mapper, IServiceLogs serviceLogs):base(authorRepository, mapper, serviceLogs)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

    }
}
