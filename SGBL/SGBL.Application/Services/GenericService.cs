using AutoMapper;
using SGBL.Application.Dtos.Role;
using SGBL.Application.Interfaces;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TDto> where TEntity : class
        where TDto : class
    {
        private readonly  IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        private string _entity = typeof(TEntity).Name;

        public IUserStatusService UserStatusService { get; }

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper, IServiceLogs serviceLogs)
        {
            _repository = repository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
            UserStatusService = default!; // <- “le digo al compilador que confíe en mí”
        }

        public GenericService(IUserStatusService userStatusService, IMapper mapper, IServiceLogs serviceLogs)
        {
            UserStatusService = userStatusService;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
            _repository = default!;
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
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la creacion de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<TDto?> UpdateAsync(TDto dto, int id)
        {           
            try
            {
                _serviceLogs.CreateLogInfo($"Actualización un/una {_entity} iniciada.");
                var entity = _mapper.Map<TEntity>(dto);
                var updated = await _repository.UpdateAsync(id, entity);
                return updated is null ? null : _mapper.Map<TDto>(updated);

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la actualización de {_entity}, " + ex);
                throw;
            }

        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
       
            try
            {
                _serviceLogs.CreateLogInfo($"Eliminación de un/una  {_entity}  iniciada.");
                await _repository.DeleteAsync(id);
                return true;


            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la eliminación de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<TDto?> GetById(int id)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"´Busquedad por id {_entity}  iniciada.");

                var entity = await _repository.GetById(id);
                return entity is null ? null : _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa en la capa Aplication con la busquedad por id de  un/una {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<List<TDto>> GetAll()
        {          
            try
            {
                _serviceLogs.CreateLogInfo($"Busquedad asincrónica de una lista de {_entity}  iniciada.");
                
                var list = await _repository.GetAllAsync();
                return _mapper.Map<List<TDto>>(list);

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la obtención de la lista de {_entity}, " + ex);
                throw;
            }
        }
        public async Task<RoleDto?> GetByIdFast(int id)
        {

            try
            {
                _serviceLogs.CreateLogInfo($"Busqueda por Id  asincrónica sin tracking de un/una {_entity} iniciada.");
                var entity = await _repository.GetByIdNoTrackingAsync(id);
                return entity is null ? null : _mapper.Map<RoleDto>(entity);

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error en en la capa Aplication con la busquedad  por id asincrónica con no tracking de un/una {_entity}, " + ex);
                throw;
            }
        }

    }
}