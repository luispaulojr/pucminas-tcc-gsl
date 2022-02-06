using System.Collections.Generic;
using System.Threading.Tasks;
using config.database.model;

namespace config.database.PostgresSQL.interfaces
{
    public interface IPostgreSQLHelper
    {
        T QuerySingle<T>(string query, object param = null);
        List<T> Query<T>(string query, object param = null);
        IEnumerable<T> QueryAsync<T>(string query);
        PaginatedList<T> QueryPaginated<T>(string query, object param, int? page, int pageSize);
        void Execute(string query, object paramList = null);
        Task<int> ExecuteAsync(string query, object paramList = null);
        T ExecuteScalar<T>(string query);
    }
}