
using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class GenreService : GenericService<Genre,GenreDto>, IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        public GenreService(IGenreRepository genreRepository, IMapper mapper, IServiceLogs serviceLogs) : base(genreRepository, mapper, serviceLogs)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }
    }
}
