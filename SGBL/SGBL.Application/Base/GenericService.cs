

using AutoMapper;
using SGBL.Application.Interfaces;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Base
{
    public class GenericService<Entity, DtoModel> : IGenericService<DtoModel>
                where DtoModel : class
                where Entity : class
    {

        private readonly IGenericRepository<Entity> _genericRepository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<Entity> genericRepository, IMapper mapper)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public virtual async Task<DtoModel?> AddAsync(DtoModel dto)
        {
            try
            {
                Entity entity = _mapper.Map<Entity>(dto);
                Entity? returnEntity = await _genericRepository.AddAsync(entity);
                if (returnEntity == null)
                {
                    return null;
                }

                return _mapper.Map<DtoModel>(returnEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual async Task<DtoModel?> UpdateAsync(DtoModel dto, int id)
        {
            try
            {
                Entity entity = _mapper.Map<Entity>(dto);
                Entity? returnEntity = await _genericRepository.UpdateAsync(id, entity);
                if (returnEntity == null)
                {
                    return null;
                }

                return _mapper.Map<DtoModel>(returnEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await _genericRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<DtoModel?> GetById(int id)
        {
            try
            {
                var entity = await _genericRepository.GetById(id);
                if (entity == null)
                {
                    return null;
                }

                DtoModel dto = _mapper.Map<DtoModel>(entity);
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public virtual async Task<List<DtoModel>> GetAll()
        {
            try
            {
                var listEntities = await _genericRepository.GetAllAsync();
                var listEntityDtos = _mapper.Map<List<DtoModel>>(listEntities);

                return listEntityDtos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
