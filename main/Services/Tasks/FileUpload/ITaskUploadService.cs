namespace TaskManager.Services.Tasks.FileUpload
{
    public interface ITaskUploadService
    {
        Task<(bool Success, string ErrorMessage, int Count)> ProcessExcelAsync(IFormFile file);
    }

}

