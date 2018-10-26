using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Base;
using BlackJack.Exceptions.DataAccessExceptions;
using BlackJack.Shared.Constants;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Generics
{
    public abstract class EFBaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected GameContext _context;
        protected DbSet<T> _dbSet;

        public EFBaseRepository(GameContext context)
        {
            _context = context;

            _dbSet = _context.Set<T>();
        }

        public async Task Create(T item)
        {
            if (item != null)
            {
                _dbSet.Add(item);

                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateMultiple(List<T> items)
        {
            if (items != null)
            {
                _dbSet.AddRange(items);

                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            T item = await _dbSet.FindAsync(id);

            if (item != null)
            {
                var dbEntry = _context.Entry(item);

                dbEntry.State = EntityState.Deleted;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<T> Get(int id)
        {
            T item = await _dbSet.FindAsync(id);

            return item;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IEnumerable<T> items = await _dbSet.ToListAsync();

            return items;
        }

        public async Task Update(T item)
        {
            if (item != null)
            {
                var dbEntry = _context.Entry(item);

                if (dbEntry == null)
                {
                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine(string.Format("ItemId: {0}", item.Id));
                    stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE));

                    string message = stringBuilder.ToString();

                    throw new DataBaseUpdateItemException(message);
                }

                dbEntry.State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateMultiple(List<T> items)
        {
            foreach (T item in items)
            {
                var dbEntry = default(DbEntityEntry<T>);
                
                if (item != null)
                {
                    dbEntry = _context.Entry(item);
                }

                if (dbEntry == null)
                {
                    var stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine(string.Format("ItemId: {0}", item.Id));
                    stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE));

                    string message = stringBuilder.ToString();

                    throw new DataBaseUpdateItemException(message);
                }

                dbEntry.State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }
    }
}