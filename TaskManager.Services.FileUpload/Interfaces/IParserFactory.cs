using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Factories;

namespace TaskManager.Services.FileUpload.Interfaces
{
    public interface IParserFactory
    {
        ITaskDataParser GetParser(string fileExtension);
    }
}



