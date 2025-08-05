using TaskManager.Services.FileUpload.Models;

namespace TaskManager.DTOs
{
    public class TaskParseResult
    {
        public bool Success { get; set; }
        public List<ParsedTask> ParsedTasks { get; set; }
        public string ErrorMessage { get; set; }
    }
}





