using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Data;
using Serilog;

namespace POC.OrderingService.Query.Repositories
{
    internal class BaseReadModelRepository<T> where T : class
    {
        protected readonly IDbConnection _dbConnection;
        private readonly ReadModelDBContext _readModelDbContext;
        public BaseReadModelRepository(IDbConnection dbConnection, ReadModelDBContext readModelDbContext)
        {
            Log.Information("BaseReadModelRepository initialized with DbConnection: {DbConnection} and ReadModelDBContext: {ReadModelDBContext}", dbConnection, readModelDbContext);
            _dbConnection = dbConnection;
            _readModelDbContext = readModelDbContext;
        }


        /// <summary>
        /// Generates the table name based on the type of the entity.
        /// Table Name calculated in single source of truth manner, so that it can be used in multiple places.
        /// </summary>
        /// <typeparam name="V">Entity Type</typeparam>
        /// <returns>table Name</returns>
        protected static string TableName<V>() => $"{typeof(V).Name}s";

        public virtual async Task<T?> GetAsync(Guid id)
        {
            return await _readModelDbContext.Set<T>().FindAsync(id);
        }
        public async Task InsertAsync(T entity)
        {
            await _readModelDbContext.Set<T>().AddAsync(entity);
            await _readModelDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _readModelDbContext.Set<T>().Update(entity);
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}
