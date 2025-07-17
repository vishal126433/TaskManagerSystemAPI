
using ExcelDataReader;
using System.Data;
using Microsoft.AspNetCore.Http;
using TaskManager.Services.FileUpload.FileUploads;
using TaskManager.Services.FileUpload.Models;


namespace TaskManager.Services.FileUpload.FileUploads
{
    public class ExcelTaskDataParser : ITaskDataParser
    {
        public Task<List<ParsedTask>> ParseAsync(IFormFile file)
        {
            var tasks = new List<ParsedTask>();

            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            DataTable table = validateFile(result);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                tasks.Add(new ParsedTask
                {
                    Name = row["name"]?.ToString() ?? "",
                    Description = row["description"]?.ToString() ?? "",
                    Duedate = DateTime.TryParse(row["dueDate"]?.ToString(), out var date) ? date : null,
                    Status = row["status"]?.ToString() ?? "",
                    Type = row["type"]?.ToString() ?? "",
                    Priority = row["priority"]?.ToString() ?? "",
                    UserId = Convert.ToInt32(row["userId"])
                });
            }

            return Task.FromResult(tasks);  //  Return wrapped in a Task

            static DataTable validateFile(DataSet result)
            {
                var table = result.Tables[0];
                if (table == null || table.Rows.Count == 0)
                    throw new Exception("Empty Excel file.");

                var expectedHeaders = new[] { "id", "Name", "Description", "DueDate", "Status", "Type", "Priority", "UserId" };
                foreach (var header in expectedHeaders)
                {
                    if (!table.Columns.Contains(header))
                        throw new Exception($"Missing expected header: {header}");
                }

                return table;
            }
        }

    }
}

