using System.Collections.Generic;
using System.IO;

namespace LogDecrypt
{
    public class FileHelper:IFileHelper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }
    }
}