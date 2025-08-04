using System.Data.Common;
using System.Data;
using Sap.Data.Hana;

namespace CvsServiceLayer.Data
{
    public class HanaDbHelper
    {

        private readonly string _connectionString;
        public HanaDbHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("HanaDb");
        }

        public async Task<List<Dictionary<string, object>>> QueryAsync(string sql)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new HanaConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new HanaCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Add(row);
            }

            return result;
        }

        public async Task<int> ExecuteAsync(string sql)
        {
            using var conn = new HanaConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new HanaCommand(sql, conn);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteProcedureAsync(string procedureName, params HanaParameter[] parameters)
        {
            using var conn = new HanaConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new HanaCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Dictionary<string, object>>> ExecuteProcedureAsyncData(string procedureName, params HanaParameter[] parameters)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new HanaConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new HanaCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters); ;

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                result.Add(row);
            }

            return result;
        }

        

        public static List<Dictionary<string, object>> ToDictionaryList(DbDataReader reader)
        {
            var list = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                list.Add(row);
            }

            return list;
        }
    }
}
