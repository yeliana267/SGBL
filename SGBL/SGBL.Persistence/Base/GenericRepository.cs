using Microsoft.EntityFrameworkCore;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Base
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly SGBLContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(SGBLContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            // Si quieres forzar split query global en este repo:
            // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<List<TEntity>?> AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public virtual async Task<TEntity?> UpdateAsync(int id, TEntity entity)
        {
            var entry = await _dbSet.FindAsync(id);

            if (entry != null)
            {
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return entry;
            }
            return null;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<TEntity?> GetById(int id)
        {
            // Si es lectura pura y no vas a modificar, puedes usar AsNoTracking
            // return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => /* filtra por id */);
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            // ⚠️ Aquí es donde te daba timeout: usa AsNoTracking para aligerar
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsyncWithInclude(List<string> properties)
        {
            // ⚠️ Importante: reasignar el query al llamar Include
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            foreach (var property in properties)
                query = query.Include(property);

            // (Opcional) Si incluyes varias colecciones, puedes dividir la consulta:
            // query = query.AsSplitQuery();

            return await query.ToListAsync();
        }

        public virtual IQueryable<TEntity> GetAllQueryWithInclude(List<string> properties)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            foreach (var property in properties)
                query = query.Include(property);

            // (Opcional) .AsSplitQuery()
            return query;
        }

        public virtual IQueryable<TEntity> GetAllQuery()
        {
            return _dbSet.AsNoTracking();
        }

        // ✅ Útil si luego quieres paginar
        public virtual async Task<List<TEntity>> GetPageAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            return await _dbSet
                .AsNoTracking()
                .OrderBy(e => 1) // Reemplaza por .OrderBy(e => e.Id) si lo tienes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // en GenericRepository<TEntity>
        public async Task<TEntity?> GetByIdNoTrackingAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

    }
}
