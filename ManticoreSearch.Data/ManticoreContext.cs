using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace ManticoreSearch.Data
{
    public class ManticoreContext(IConfiguration config)
    {
        public async Task<List<T>> GetDataAsync<T>(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            using(var connection = new MySqlConnection(config.GetConnectionString("Manticore")))
            {
                var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
                var rows = await connection.QueryAsync<T>(command);
                return rows.ToList();
            }
        }
        public async Task<List<dynamic>> GetDataAsync(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            using (var connection = new MySqlConnection(config.GetConnectionString("Manticore")))
            {
                var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
                var rows = await connection.QueryAsync(command);
                return rows.ToList();
            }
        }

        public async Task ExecuteSqlAsync(string sql, object? parameters = null, CancellationToken cancellation = default)
        {
            using (var connection = new MySqlConnection(config.GetConnectionString("Manticore")))
            {
                var command = new CommandDefinition(sql, parameters, cancellationToken: cancellation);
                await connection.ExecuteAsync(command);
            }
        }
    }


}
