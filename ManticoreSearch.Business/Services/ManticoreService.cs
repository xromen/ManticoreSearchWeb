using Dapper;
using ManticoreSearch.Business.DTOs;
using ManticoreSearch.Business.Models;
using ManticoreSearch.Data;
using System.Dynamic;

namespace ManticoreSearch.Business.Services
{
    public class ManticoreService(ManticoreContext context)
    {
        public async Task<List<Table>> GetAllTablesAsync(CancellationToken cancellation = default)
        {
            string sql = "show tables;";

            var tableDtos = await context.GetDataAsync<TableDto>(sql, cancellation: cancellation);

            var tables = new List<Table>();

            foreach (var table in tableDtos)
            {
                var columns = await GetTableColumnsAsync(table.Index, cancellation);

                tables.Add(table.ToModel(columns));
            }

            return tables;
        }

        public async Task<Table> GetTableAsync(string name, CancellationToken cancellation = default)
        {
            string sql = "show tables like @name;";

            var tableDto = (await context.GetDataAsync<TableDto>(sql, new { name }, cancellation)).FirstOrDefault();

            if(tableDto == null)
            {
                throw new Exception("Таблица не найдена.");
            }

            var columns = await GetTableColumnsAsync(name, cancellation);

            return tableDto.ToModel(columns);
        }

        public async Task<List<dynamic>> GetTableDataAsync(Table table, CancellationToken cancellation = default)
        {
            string sql = $"select * from {table.Name}";

            var data = await context.GetDataAsync<dynamic>(sql, cancellation);

            return data.ToList();
        }

        public async Task<List<Column>> GetTableColumnsAsync(string tableName, CancellationToken cancellation = default)
        {
            string sql = $@"desc {tableName};";

            var columns = await context.GetDataAsync<ColumnDto>(sql, cancellation: cancellation);

            return columns.OrderBy(c => c.Id).Select(c => c.ToModel()).ToList();
        }

        public async Task<bool> DeleteTableAsync(Table table, CancellationToken cancellation = default)
        {
            string sql = $@"drop table {table.Name};";

            var existedTable = await GetTableAsync(table.Name, cancellation);

            await context.ExecuteSqlAsync(sql, cancellation);

            return true;
        }
    }
}
