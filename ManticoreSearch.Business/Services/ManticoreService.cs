using Dapper;
using ManticoreSearch.Business.DTOs;
using ManticoreSearch.Business.Models;
using ManticoreSearch.Data;

namespace ManticoreSearch.Business.Services
{
    public class ManticoreService(ManticoreContext context)
    {
        public async Task<List<Table>> GetTablesAsync(CancellationToken cancellation = default)
        {
            string sql = "show tables;";

            var tableDtos = await context.GetData<TableDto>(sql, cancellation: cancellation);

            var tables = new List<Table>();

            foreach (var table in tableDtos)
            {
                var columns = await GetColumnsAsync(table, cancellation);

                tables.Add(new Table()
                {
                    Name = table.Index,
                    Columns = columns
                });
            }

            return tables;
        }

        public async Task<List<Column>> GetColumnsAsync(TableDto table, CancellationToken cancellation = default)
        {
            string sql = $@"desc {table.Index};";

            var columns = await context.GetData(sql, cancellation: cancellation);

            return columns.OrderBy(c => c.Id)
                .Select(c => new Column
                {
                    Name = c.Field,
                    Type = c.Type
                })
                .ToList();
        }
    }
}
