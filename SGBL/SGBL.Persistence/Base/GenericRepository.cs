using Microsoft.EntityFrameworkCore;
using SGBL.Application.Interfaces;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Base
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly SGBLContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IServiceLogs _serviceLogs;
        private string _entity = typeof(TEntity).Name;
        public GenericRepository(SGBLContext context, IServiceLogs serviceLogs)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _serviceLogs = serviceLogs;

        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            
            try
            {
               
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                _serviceLogs.CreateLogInfo($"Creación de  un/una {_entity} correctamente completado/a.");
                return entity;


            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la creación de un/una {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<List<TEntity>?> AddRangeAsync(List<TEntity> entities)
        {            
         
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                _serviceLogs.CreateLogInfo($"Creación de una lista de {_entity} correctamente completado/a.");
                return entities;

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la creación de la lista de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<TEntity?> UpdateAsync(int id, TEntity entity)
        {

            try
            {
                _serviceLogs.CreateLogInfo($"Busquedad por id de un/una {_entity} iniciada.");

                var entry = await _dbSet.FindAsync(id);
                _serviceLogs.CreateLogInfo($"Busquedad por id de un/una {_entity} terminida.");
                if (entry != null)
                {
                    _context.Entry(entry).CurrentValues.SetValues(entity);
                    await _context.SaveChangesAsync();
                    _serviceLogs.CreateLogInfo($"Actualización de un/una {_entity} correctamente completado/a.");

                    return entry;
                }
                _serviceLogs.CreateLogInfo($" La/el {_entity} no se pudo actualizar debido, a que el resultado de la busqueda de este via id, es null.");

                return null;
               
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la actualización de un/una {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task DeleteAsync(int id)
        {           
            try
            {
                _serviceLogs.CreateLogInfo($"Busquedad de un/una {_entity} iniciada.");
                var entity = await _dbSet.FindAsync(id);
                _serviceLogs.CreateLogInfo($"Busquedad de un/una {_entity} terminada.");
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    _serviceLogs.CreateLogInfo($"Eliminacíon de un/una {_entity} correctamente completado/a.");

                }
                else {
                    _serviceLogs.CreateLogInfo($"Eliminacíon de un/una {_entity} no se pudo completar debido a que el resultado de la busqueda de este via id, es null.");

                }                 

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en  la capa Persistence con la eliminación de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<TEntity?> GetById(int id)
        {
            // Si es lectura pura y no vas a modificar, puedes usar AsNoTracking
            // return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => /* filtra por id */);
       
            try
            {
                var entity = await _dbSet.FindAsync(id);
                _serviceLogs.CreateLogInfo($"Busquedad via id de un/una {_entity} correctamente completado/a.");

                return entity;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la busquedad de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            // ⚠️ Aquí es donde te daba timeout: usa AsNoTracking para aligerar
          
            try
            {
                var   asyncList = await _dbSet
                .AsNoTracking()
                .ToListAsync();
                _serviceLogs.CreateLogInfo($"Busqueda de lista asincrónica de un/una {_entity} correctamente completado/a.");

                return asyncList;

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la busquedad asincrónica de la lista de {_entity}, " + ex);
                throw;
            }
        }

        public virtual async Task<List<TEntity>> GetAllAsyncWithInclude(List<string> properties)
        {
          
            try
            {

                // ⚠️ Importante: reasignar el query al llamar Include
                IQueryable<TEntity> query = _dbSet.AsNoTracking();

                foreach (var property in properties)
                    query = query.Include(property);

                // (Opcional) Si incluyes varias colecciones, puedes dividir la consulta:
                // query = query.AsSplitQuery();
                var queryResult = await query.ToListAsync();
                _serviceLogs.CreateLogInfo($"Busqueda de lista asincrónica con include de un/una {_entity} correctamente completado/a.");

                return queryResult;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la busquedad asincrónica con inlclued de la lista de {_entity}, " + ex);
                throw;
            }
        }

        public virtual IQueryable<TEntity> GetAllQueryWithInclude(List<string> properties)
        {
           
            try
            {
                IQueryable<TEntity> query = _dbSet.AsNoTracking();
                foreach (var property in properties)
                    query = query.Include(property);
                _serviceLogs.CreateLogInfo($"Obtener query con include de un/una {_entity} correctamente completado/a.");

                // (Opcional) .AsSplitQuery()
                return query;

            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la obtencion de la query con include de {_entity}, " + ex);
                throw;
            }
        }

        public virtual IQueryable<TEntity> GetAllQuery()
        {
          
            try
            {

                _serviceLogs.CreateLogInfo($"creacion de consulta sin tracking para {_entity} iniciada.");
                var queryResult = _dbSet.AsNoTracking();
                _serviceLogs.CreateLogInfo($"creacion de consulta sin tracking para {_entity} terminada.");

                return queryResult;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la creacion de consulta sin tracking de {_entity}, " + ex);
                throw;
            }
        }

        // Útil si luego quieres paginar
        public virtual async Task<List<TEntity>> GetPageAsync(int page, int pageSize)
        {
          
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 20;

                _serviceLogs.CreateLogInfo($"Obtención de pagina asincrónica de  un/una {_entity} iniciado.");
                var result=  await _dbSet
                    .AsNoTracking()
                    .OrderBy(e => 1) // Reemplaza por .OrderBy(e => e.Id) si lo tienes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                _serviceLogs.CreateLogInfo($"Obtención de pagina asincrónica de  un/una {_entity} correctamente completado/a.");

                return result;


            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la optencion de pagina asincrónica de un  {_entity}, " + ex);
                throw;
            }
        }
        // en GenericRepository<TEntity>
        public async Task<TEntity?> GetByIdNoTrackingAsync(int id)
        {
           
            try
            {
                var result= await _dbSet.AsNoTracking()
               .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);

                _serviceLogs.CreateLogInfo($"busqueda por Id  asincrónica sin tracking de un/una {_entity} correctamente completado/a.");
                return result;


            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogError($"Error en en la capa Persistence con la busqueda de  un/una {_entity} asincrónica por Id con no tracking, " + ex);
                throw;
            }
        }

    }
}
