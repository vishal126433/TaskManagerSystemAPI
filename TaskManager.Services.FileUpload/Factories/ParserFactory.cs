using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Services.FileUpload.Parsers;



namespace TaskManager.Services.FileUpload.Factories
{
    public class ParserFactory : IParserFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ParserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITaskDataParser GetParser(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".xlsx" or ".xls" => (ITaskDataParser)_serviceProvider.GetService(typeof(ExcelTaskDataParser)),
                // add future parsers here
                _ => throw new NotSupportedException("Unsupported file type")
            };
        }
    }
}
