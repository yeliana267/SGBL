using AutoMapper;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Interfaces;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TDto> where TEntity : class
        where TDto : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;

        public IUserStatusService UserStatusService { get; }

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public GenericService(IUserStatusService userStatusService, IMapper mapper)
        {
            UserStatusService = userStatusService;
            _mapper = mapper;
        }

        public virtual async Task<TDto?> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            var saved = await _repository.AddAsync(entity);
            return _mapper.Map<TDto>(saved);
        }

        public virtual async Task<TDto?> UpdateAsync(TDto dto, int id)
        {
            var entity = _mapper.Map<TEntity>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            return updated is null ? null : _mapper.Map<TDto>(updated);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public virtual async Task<TDto?> GetById(int id)
        {
            var entity = await _repository.GetById(id);
            return entity is null ? null : _mapper.Map<TDto>(entity);
        }

        public virtual async Task<List<TDto>> GetAll()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<List<TDto>>(list);
        }
        public async Task<RoleDto?> GetByIdFast(int id)
        {
            var entity = await _repository.GetByIdNoTrackingAsync(id);
            return entity is null ? null : _mapper.Map<RoleDto>(entity);
        }

    }
}