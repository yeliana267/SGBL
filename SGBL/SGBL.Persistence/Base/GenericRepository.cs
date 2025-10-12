﻿
using Microsoft.EntityFrameworkCore;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Base
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        private readonly SGBLContext _context;
        public GenericRepository(SGBLContext context)
        {
            _context = context;
        }
        public virtual async Task<Entity> AddAsync(Entity entity)
        {
            await _context.Set<Entity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<List<Entity>?> AddRangeAsync(List<Entity> entities)
        {
            await _context.Set<Entity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public virtual async Task<Entity?> UpdateAsync(int id, Entity entity)
        {
            var entry = await _context.Set<Entity>().FindAsync(id);

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
            var entity = await _context.Set<Entity>().FindAsync(id);

            if (entity != null)
            {
                _context.Set<Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<Entity?> GetById(int id)
        {
            return await _context.Set<Entity>().FindAsync(id);
        }

        public virtual async Task<List<Entity>> GetAllAsync()
        {
            return await _context.Set<Entity>().ToListAsync();
        }

        public virtual async Task<List<Entity>> GetAllAsyncWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var property in properties)
            {
                query.Include(property);
            }
            return await query.ToListAsync();
        }

        public virtual IQueryable<Entity> GetAllQueryWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var property in properties)
            {
                query.Include(property);
            }
            return query;
        }
        public virtual IQueryable<Entity> GetAllQuery()
        {
            return _context.Set<Entity>().AsQueryable();
        }
    }
}
