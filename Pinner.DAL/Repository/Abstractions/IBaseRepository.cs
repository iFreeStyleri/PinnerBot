using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinner.DAL.Entities;

namespace Pinner.DAL.Repository.Abstractions
{
    public interface IBaseRepository<T>: IDisposable where T : Entity
    {
        Task<T?> GetAsync(int id);

        IQueryable<T> GetAll();
        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Add(T entity);
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        void Update(T entity);
        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        void Remove(T entity);
        /// <summary>
        /// Add and save changes entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        Task AddAsync(T entity);
        /// <summary>
        /// Remove and save changes entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task RemoveAsync(T entity);
        /// <summary>
        /// Update and save changes entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        Task UpdateAsync(T entity);
        
        Task SaveChangesAsync();

        Task<T?> GetRandomRow();
    }
}
