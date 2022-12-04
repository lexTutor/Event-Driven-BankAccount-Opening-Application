using BankAccount.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Shared.Data
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly DbContext _context;
        private DbSet<T> _entities;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public GenericRepository(BankContext context)
        {
            _context = context;
        }

        #endregion

        #region Properties

        //<inheritdoc/>
        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        //<inheritdoc/>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }

        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        //<inheritdoc/>
        public virtual DbContext DbContext => _context;

        #endregion

        #region EfRepository.Implementations

        //<inheritdoc/>
        public virtual T GetById(object id)
        {
            return Entities.Find(id);
        }

        //<inheritdoc/>
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await Entities.FindAsync(id);
        }

        //<inheritdoc/>
        public async virtual Task InsertAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                await Entities.AddAsync(entity);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual T SaveOrUpdate(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _context.Update(entity);
                _context.SaveChanges();

                return entity;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual async Task<T> SaveOrUpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _context.Update(entity);
                await _context.SaveChangesAsync();

                return entity;
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual void SaveOrUpdate(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                _context.Update(entities);
                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual async Task SaveOrUpdateAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                _context.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(entity);

                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual async Task DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    Entities.Remove(entity);

                _context.SaveChanges();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        //<inheritdoc/>
        public virtual async Task DeleteAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    Entities.Remove(entity);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        private string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            //rollback entity changes
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    entry.State = EntityState.Unchanged;
                });
            }

            try
            {
                _context.SaveChanges();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                //if after the rollback of changes the context is still not saving,
                //return the full text of the exception that occurred when saving
                return ex.ToString();
            }
        }

        #endregion
    }
}
