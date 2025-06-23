using AuthService.Data;
using System.IO;

namespace TaskManager.Services.Tasks.FileUpload
{
    public class TaskUploadService : ITaskUploadService
    {
        private readonly AppDbContext _db;
        private readonly ITaskDataParser _parser;

        public TaskUploadService(AppDbContext db, ITaskDataParser parser)
        {
            _db = db;
            _parser = parser;
        }

        public async Task<(bool Success, string ErrorMessage, int Count)> ProcessExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "No file uploaded.", 0);

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".xlsx" || extension == ".xls")
                {
                    var tasks = await _parser.ParseAsync(file);
                    _db.Tasks.AddRange(tasks);
                    await _db.SaveChangesAsync();
                    return (true, "", tasks.Count);
                }
                else if (extension == ".csv")
                {
                    return (false, "CSV parsing not implemented.", 0);
                }
                else
                {
                    return (false, "Unsupported file type.", 0);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }
    }
}
