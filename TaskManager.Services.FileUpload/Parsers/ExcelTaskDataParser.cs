using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using TaskManager.Services.FileUpload.Models;
using TaskManager.Services.FileUpload.Interfaces;
using FileParser.Models;

namespace TaskManager.Services.FileUpload.Parsers
{
    public class ExcelTaskDataParser : ITaskDataParser
    {
        private readonly ILogger<ExcelTaskDataParser> _logger;

        public ExcelTaskDataParser(ILogger<ExcelTaskDataParser> logger)
        {
            _logger = logger;
        }

        public Task<List<ParsedTask>> ParseAsync(IFormFile file)
        {
            var tasks = new List<ParsedTask>();

            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file uploaded.");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var stream = file.OpenReadStream();
                using var reader = ExcelReaderFactory.CreateReader(stream);

                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                });

                DataTable table = ValidateFile(result);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    tasks.Add(new ParsedTask
                    {
                        Name = row[ExcelHeaders.Name]?.ToString() ?? "",
                        Description = row[ExcelHeaders.Description]?.ToString() ?? "",
                        Duedate = DateTime.TryParse(row[ExcelHeaders.DueDate]?.ToString(), out var date) ? date : null,
                        Status = row[ExcelHeaders.Status]?.ToString() ?? "",
                        Type = row[ExcelHeaders.Type]?.ToString() ?? "",
                        Priority = row[ExcelHeaders.Priority]?.ToString() ?? "",
                    });
                }

                return Task.FromResult(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing Excel file: {FileName}", file?.FileName);
                throw; // rethrow for higher-level handlers
            }
        }

        private static DataTable ValidateFile(DataSet result)
        {
            var table = result.Tables[0];
            if (table == null || table.Rows.Count == 0)
                throw new Exception("Empty Excel file.");

            var expectedHeaders = new[]
            {
                ExcelHeaders.Id,
                ExcelHeaders.Name,
                ExcelHeaders.Description,
                ExcelHeaders.DueDate,
                ExcelHeaders.Status,
                ExcelHeaders.Type,
                ExcelHeaders.Priority,
                ExcelHeaders.AssignTo
            };

            foreach (var header in expectedHeaders)
            {
                if (!table.Columns.Contains(header))
                    throw new Exception($"Missing expected header: {header}");
            }

            return table;
        }
    }
}
