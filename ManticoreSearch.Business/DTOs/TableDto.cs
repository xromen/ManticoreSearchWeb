using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManticoreSearch.Business.DTOs
{
    public class TableDto
    {
        public required string Index { get; set; }
        public required string Type { get; set; }
    }
}
