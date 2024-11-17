using Dapper;
using MySql.Data.MySqlClient;

namespace ManticoreSearch.Data
{
    public class ManticoreContext(MySqlConnection connection)
    {
        public async Task<List<T>> GetData<T>(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
            var rows = await connection.QueryAsync<T>(command);
            return rows.ToList();
        }
        public async Task<List<dynamic>> GetData(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
            var rows = await connection.QueryAsync(command);
            return rows.ToList();
        }

        public Task ExecuteSql(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
            return connection.ExecuteAsync(command);
        }
    }


}
