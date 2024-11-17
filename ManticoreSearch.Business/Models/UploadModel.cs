using ManticoreSearch.Business.Enums;
using Microsoft.AspNetCore.Components.Forms;
using System.Data;

namespace ManticoreSearch.Business.Models
{
    public class UploadModel
    {
        public IBrowserFile? File { get; set; }
        public FileTypes.FileType? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? TableName { get; set; }
        public bool NewTable { get; set; }
        public string? CsvValueDivider { get; set; }
        public bool FirstRowHeader { get; set; }
        public DataTable? ExampleTable { get; set; }
        public static UploadModel GetDefault()
        {
            return new UploadModel()
            {
                File = null,
                FileType = null,
                FilePath = null,
                TableName = null,
                NewTable = false,
                CsvValueDivider = ",",
                FirstRowHeader = true,
                ExampleTable = null
            };
        }
    }
}
