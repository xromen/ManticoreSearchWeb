namespace ManticoreSearch.Business.Models
{
    public class Table
    {
        public required string Name { get; set; }
        public List<Column> Columns { get; set; } = new();
    }
}
