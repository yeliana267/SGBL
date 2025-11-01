using AutoMapper;
using SGBL.Application.Interfaces;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TDto>
        where TEntity : class
        where TDto : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        private readonly string _entity = typeof(TEntity).Name;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper, IServiceLogs serviceLogs)
        {
            _repository = repository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
        }

        public virtual async Task<TDto?> AddAsync(TDto dto)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Creación de un/una {_entity} iniciada.");
                var entity = _mapper.Map<TEntity>(dto);
                var saved = await _repository.AddAsync(entity);
                return _mapper.Map<TDto>(saved);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error creando {_entity}: {ex}");
                throw;
            }
        }

        public virtual async Task<TDto?> UpdateAsync(TDto dto, int id)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Actualización de {_entity} iniciada.");
                var entity = _mapper.Map<TEntity>(dto);
                var updated = await _repository.UpdateAsync(id, entity);
                return updated is null ? null : _mapper.Map<TDto>(updated);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error actualizando {_entity}: {ex}");
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Eliminación de {_entity} iniciada.");
                await _repository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error eliminando {_entity}: {ex}");
                throw;
            }
        }

        public virtual async Task<TDto?> GetById(int id)
        {
            try
            {
                var entity = await _repository.GetById(id);
                return entity is null ? null : _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al buscar {_entity} por ID: {ex}");
                throw;
            }
        }

        public virtual async Task<List<TDto>> GetAll()
        {
            try
            {
                var list = await _repository.GetAllAsync();
                return _mapper.Map<List<TDto>>(list);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al listar {_entity}: {ex}");
                throw;
            }
        }

        public virtual async Task<TDto?> GetByIdFast(int id)
        {
            try
            {
                var entity = await _repository.GetByIdNoTrackingAsync(id);
                return entity is null ? null : _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al buscar (sin tracking) {_entity}: {ex}");
                throw;
            }
        }
    }
}
