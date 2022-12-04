using Microsoft.EntityFrameworkCore;

namespace BankAccount.Shared.Contracts
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }

        /// <summary>
        /// The context of the implementing class
        /// </summary>
        DbContext DbContext { get; }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>T</returns>
        T GetById(object id);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>T</returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// Inserts or updates a given entity to the database 
        /// depending on the value of the primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// Asynchronously inserts or updates a given entity to the database 
        /// depending on the value of the primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        Task<T> SaveOrUpdateAsync(T entity);

        /// <summary>
        /// Inserts or updates a collection of entities to the database 
        /// depending on the value of the primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        void SaveOrUpdate(IEnumerable<T> entities);

        /// <summary>
        /// Asynchronously inserts or updates a collection of entities to the database 
        /// depending on the value of the primary key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        Task SaveOrUpdateAsync(IEnumerable<T> entities);

        /// <summary>
        /// Deletes an entity from the database
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// Asynchronously deletes an entity from the database
        /// </summary>
        /// <param name="entity">Entity</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Deletes a collection of entities from the database
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Asynchronously deletes a collection of entities from the database
        /// </summary>
        /// <param name="entity">Entity</param>
        Task DeleteAsync(IEnumerable<T> entities);

        /// <summary>
        /// Inserts an entity into the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        Task InsertAsync(T entity);
    }

}
