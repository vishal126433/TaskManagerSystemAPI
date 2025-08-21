using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Services.FileUpload.Models;

namespace TaskManager.Services.FileUpload.Services
{
    public class TaskUploadService : ITaskUploadService
    {
        private readonly IParserFactory _parserFactory;
        private readonly ILogger<TaskUploadService> _logger;

        public TaskUploadService(IParserFactory parserFactory, ILogger<TaskUploadService> logger)
        {
            _parserFactory = parserFactory;
            _logger = logger;
        }

        public async Task<List<ParsedTask>> ParseTasksFromFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file uploaded.");

                var extension = Path.GetExtension(file.FileName);
                var parser = _parserFactory.GetParser(extension);

                if (parser == null)
                    throw new NotSupportedException($"No parser available for the file type '{extension}'.");

                var parsedTasks = await parser.ParseAsync(file);

                return parsedTasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while parsing the file: {FileName}", file?.FileName);
                throw; // rethrow so middleware can handle it
            }
        }
    }
}
