using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManticoreSearch.Business.Enums
{
    public class FileTypes
    {
        public enum FileType
        {
            Excel = 1,
            CSV = 2
        }

        public static readonly Dictionary<FileType, List<string>> SupportedFilesExtensions =
            new()
            {
                [FileType.CSV] = [".csv"],
                [FileType.Excel] = [".xls", ".xlsx"],
            };

        public static string GetAccept()
        {
            return string.Join(", ", SupportedFilesExtensions.Select(c => string.Join(", ", c.Value)));
        }

        public static bool IsValid(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName).ToLower();

            return SupportedFilesExtensions.Values.Any(c => c.Contains(fileExtension));
        }

        public static FileType? Get(string fileName)
        {
            if (fileName.Contains('.') == false)
            {
                throw new Exception("Вы выбрали неправильный файл. Попробуйте выбрать другой.");
            }

            string fileExtension = Path.GetExtension(fileName).ToLower();

            FileType fileType = SupportedFilesExtensions.Where(kvp => kvp.Value.Contains(fileExtension))
                .Select(kvp => kvp.Key)
                .FirstOrDefault();

            return fileType;
        }
    }
}
