using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Base;
using BlackJack.Exceptions.DataAccessExceptions;
using BlackJack.Shared.Constants;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Generics
{
    public abstract class DapperBaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected string _connectionString;

        public DapperBaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Create(T item)
        {
            if (item != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    long itemId = await connection.InsertAsync<T>(item);

                    item.Id = Convert.ToInt32(itemId);
                }
            }
        }

        public async Task CreateMultiple(List<T> items)
        {
            if (items != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.InsertAsync(items);
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                T itemForDelete = (T)Activator.CreateInstance(typeof(T));

                itemForDelete.Id = id;

                bool isSuccess = await connection.DeleteAsync(itemForDelete);
            }
        }

        public async Task<T> Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                T item = await connection.GetAsync<T>(id);

                return item;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                IEnumerable<T> items = await connection.GetAllAsync<T>();

                return items;
            }
        }

        public async Task Update(T item)
        {
            if (item != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    bool isSuccess = await connection.UpdateAsync(item);

                    if (!isSuccess)
                    {
                        var stringBuilder = new StringBuilder();

                        stringBuilder.AppendLine(string.Format("ItemId: {0}", item.Id));
                        stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE));

                        string message = stringBuilder.ToString();

                        throw new DataBaseUpdateItemException(message);
                    }
                }
            }
        }

        public async Task UpdateMultiple(List<T> items)
        {
            if (items != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    bool isSuccess = await connection.UpdateAsync(items);

                    if (!isSuccess)
                    {
                        var stringBuilder = new StringBuilder();

                        for (int i = 0; i < items.Count; i++)
                        {
                            stringBuilder.AppendLine(string.Format("ItemId: {0}", items[i].Id));
                        }
                        
                        stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE));

                        string message = stringBuilder.ToString();

                        throw new DataBaseUpdateItemException(message);
                    }
                }
            }
        }

    }
}