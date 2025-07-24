using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Services.FileUpload.Models;

namespace TaskManager.Services.FileUpload.Services
{
    public class TaskUploadService : ITaskUploadService
    {
        private readonly IParserFactory _parserFactory;

        public TaskUploadService(IParserFactory parserFactory)
        {
            _parserFactory = parserFactory;
        }

        public async Task<(bool Success, string ErrorMessage, List<ParsedTask> ParsedTasks)> ParseTasksFromFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "No file uploaded.", null);

                var extension = Path.GetExtension(file.FileName);
                
                var parser = _parserFactory.GetParser(extension);
                if (parser == null)
                    return (false, "No parser available for this file type.", null);

                var parsedTasks = await parser.ParseAsync(file);
                return (true, "", parsedTasks);
            }
            catch (NotSupportedException ex)
            {
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }

}






















