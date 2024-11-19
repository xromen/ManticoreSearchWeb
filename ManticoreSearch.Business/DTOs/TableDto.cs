using ManticoreSearch.Business.Models;

namespace ManticoreSearch.Business.DTOs
{
    public class TableDto
    {
        public required string Index { get; set; }
        public required string Type { get; set; }

        public Table ToModel(List<Column> columns)
        {
            return new Table()
            {
                Name = this.Index,
                Columns = columns
            };
        }
    }
}
