using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using config.database.model;
using config.database.PostgresSQL.interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using Serilog;

namespace config.database.PostgresSQL.impl
{
    public class PostgreSQLHelper : IPostgreSQLHelper
    {
        private static SecretsManagerCache cache;
        private readonly string connectionString;

        public PostgreSQLHelper(IConfiguration config) => connectionString = getConnection(config);

        #region Connection String
        internal static NpgsqlConnectionStringBuilder GetConnectionStringBuilder(DatabaseConfig config)
        {
           return new NpgsqlConnectionStringBuilder
           {
               Host = config.host,
               Database = config.database,
               Username = config.username,
               Password = config.password,
               SearchPath = config.schema,
               SslMode = SslMode.Prefer,
               TrustServerCertificate = true,
               Timezone = "America/Sao_Paulo"
           };
        }

        internal static DatabaseConfig setDataConnection(DatabaseConfig database, IConfiguration config)
        {
            database.host = config["Postgres:host"];
            database.database = config["Postgres:database"];
            database.schema = config["Postgres:schema"];
        
            return database;
        }

        internal static string getConnection(IConfiguration config)
        {
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(config["AWS:Region"]));

            try
            {
                cache = new SecretsManagerCache(client);
                var obj = JsonConvert.DeserializeObject<DatabaseConfig>(cache.GetSecretString(config["AWS:SecretsManager"]).Result);

                return GetConnectionStringBuilder(setDataConnection(obj, config)).ConnectionString;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro ao tentar obter a SecretsManager. Error: {e.Message}");
                throw;
            }
        }
        #endregion Connection String

        #region  CRUD Methdos
        public void Execute(string query, object paramList = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> ExecuteAsync(string query, object paramList = null)
        {
            throw new System.NotImplementedException();
        }

        public T ExecuteScalar<T>(string query)
        {
            throw new System.NotImplementedException();
        }

        public List<T> Query<T>(string query, object param = null)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> QueryAsync<T>(string query)
        {
            throw new System.NotImplementedException();
        }

        public PaginatedList<T> QueryPaginated<T>(string query, object param, int? page, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        public T QuerySingle<T>(string query, object param = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion CRUD Methods
        
    }
    #region Data Model
    internal class DatabaseConfig
    {
        internal string host { get; set; }
        internal string username { get; set; }
        internal string password { get; set; }
        internal string database { get; set; }
        internal string schema { get; set; }
    }
    #endregion Data Model
}
