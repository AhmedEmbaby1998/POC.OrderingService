using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POC.OrderingService.Query.Data;

namespace POC.OrderingService.Query.Repositories
{
    internal class BaseReadModelRepository
    {
        protected readonly IDbConnection _dbConnection;

        public BaseReadModelRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        /// <summary>
        /// Generates the table name based on the type of the entity.
        /// Table Name calculated in single source of truth manner, so that it can be used in multiple places.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns>table Name</returns>
        protected string TableName<T>() => $"{typeof(T).Name}s";
    }
}
