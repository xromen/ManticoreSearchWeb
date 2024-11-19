using ManticoreSearch.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManticoreSearch.Business.DTOs
{
    public class ColumnDto
    {
        public required int Id { get; set; }
        public required string Field { get; set; }
        public required string Type { get; set; }
        public required string Property { get; set; }

        public Column ToModel()
        {
            return new Column
            {
                Name = this.Field,
                Type = this.Type
            };
        }
    }
}
