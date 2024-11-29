using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pinner.DAL.Entities;
using Pinner.DAL.Repository.Abstractions;

namespace Pinner.DAL.Repository.Implementations
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity
    {
        private readonly BotContext _context;
        private readonly Random _rand;

        public BaseRepository(BotContext context)
        {
            _context = context;
        }

        public async Task<T?> GetAsync(int id)
            => await _context.Set<T>()
                .SingleOrDefaultAsync(s => s.Id == id);

        public IQueryable<T> GetAll()
            => _context.Set<T>();

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public async Task AddAsync(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

        }

        public async Task<T?> GetRandomRow()
        {
            var result = await _context.Set<T>()
                .OrderBy(o => EF.Functions.Random()).AsNoTracking().FirstAsync();
            return result;
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
