
using ExcelDataReader;
using System.Data;
using AuthService.Data;

namespace TaskManager.Services.Tasks.FileUpload
{
    public class ExcelTaskDataParser : ITaskDataParser
    {
        public async Task<List<TaskItem>> ParseAsync(IFormFile file)
        {
            var tasks = new List<TaskItem>();

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
                tasks.Add(new TaskItem
                {
                    Name = row["name"]?.ToString() ?? "",
                    Description = row["description"]?.ToString() ?? "",
                    Duedate = DateTime.TryParse(row["dueDate"]?.ToString(), out var date) ? date : null,
                    Status = row["status"]?.ToString() ?? "",
                    Type = row["type"]?.ToString() ?? "",
                    UserId = Convert.ToInt32(row["userId"])
                });
            }

            return tasks;

            static DataTable validateFile(DataSet result)
            {
                var table = result.Tables[0];
                if (table == null || table.Rows.Count == 0)
                    throw new Exception("Empty Excel file.");

                var expectedHeaders = new[] { "id", "name", "description", "dueDate", "status", "type", "userId" };
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

