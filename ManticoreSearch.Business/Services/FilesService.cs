using CsvHelper;
using CsvHelper.Configuration;
using ManticoreSearch.Business.Enums;
using ManticoreSearch.Business.Models;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Threading;

namespace ManticoreSearch.Business.Services
{
    public class FilesService
    {
        private readonly string SaveFolder = @"C:\Users\User\source\repos\ManticoreMudBlazor\unsafe_uploads";
        private readonly long MaxFileSize = 2L * 1024L * 1024L * 1024L;
        private readonly int CountExampleRows = 5;
        public async Task<DataTable> GetExampleTableAsync(UploadModel model, CancellationToken cancellation = default)
        {
            if (model.File == null)
            {
                throw new Exception("Ошибка файла.");
            }

            if (model.FilePath == null)
            {
                throw new Exception("Файл не загружен. Вначале загрузите файл функцие SaveFileAsync.");
            }

            switch (model.FileType)
            {
                case FileTypes.FileType.CSV:
                    {
                        if (model.CsvValueDivider == null)
                            throw new Exception("Разделитель значений обязателен к заполнению.");

                        return await ParseCsvFileAsync(model.FilePath, model.FirstRowHeader, model.CsvValueDivider, 5, cancellation);
                    }
                default:
                    throw new Exception("Данный тип файла не поддерживается.");
            }

        }

        public async Task<string> SaveFileAsync(IBrowserFile file, IProgress<long>? progress, CancellationToken cancellation)
        {
            DirectoryInfo info = new DirectoryInfo(SaveFolder);

            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }

            FileInfo[] tempFiles = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            if (tempFiles.Count() >= 10)
            {
                for (int i = 0; i < tempFiles.Length - 10; i++)
                {
                    File.Delete(tempFiles[i].FullName);
                }
            }

            string fullFileName = Path.Combine(SaveFolder, Path.GetRandomFileName());
            using FileStream fs = new FileStream(fullFileName, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 1024, cancellation).CopyToAsync(fs, progress, cancellation);
            return fullFileName;
        }

        private async Task<DataTable> ParseCsvFileAsync(string path, bool firstRowHeader, string valueDivider, int rows = 0, CancellationToken cancellation = default)
        {
            DataTable data = new DataTable();

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.Delimiter = valueDivider;
            config.BadDataFound = null;
            config.HasHeaderRecord = firstRowHeader;
            if (rows > 0)
            {
                config.ShouldSkipRecord = args =>
                {
                    var rawRow = args.Row.Parser.RawRow;
                    return rawRow > rows + 1;
                };
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            using (var dr = new CsvDataReader(csv))
            {
                await Task.Run(() =>
                {
                    data.Load(dr);

                    if (!firstRowHeader)
                    {
                        for (int i = 0; i < data.Columns.Count; i++)
                        {
                            data.Columns[i].ColumnName = "column" + (i + 1);
                        }
                    }
                }, cancellation);
            }

            return data;
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long>? progress, CancellationToken cancellationToken, int bufferSize = 81920)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;
            long totalRead = 0;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                //Thread.Sleep(10);
                totalRead += bytesRead;
                if (progress != null)
                    progress.Report(totalRead);
            }
        }
    }
}
