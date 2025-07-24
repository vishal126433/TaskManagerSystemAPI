using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Models;

namespace TaskManager.Services.FileUpload.Interfaces
{
    public interface ITaskDataParser
    {
        Task<List<ParsedTask>> ParseAsync(IFormFile file);

    }
}


