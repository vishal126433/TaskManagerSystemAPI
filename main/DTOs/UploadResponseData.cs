using TaskManager.Services.FileUpload.Models;

namespace TaskManager.DTOs
{
    public class UploadResponseData
    {
        public string message { get; set; }
        public int count { get; set; }
        public List<ParsedTask> data { get; set; }
    }

}
