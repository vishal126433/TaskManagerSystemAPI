using Microsoft.AspNetCore.Http;
using TaskManager.Services.FileUpload.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManager.Services.FileUpload.FileUploads
{
    public interface ITaskDataParser
    {
        Task<List<ParsedTask>> ParseAsync(IFormFile file);

    }
}
