using System.Collections.Generic;

namespace LogDecrypt
{
    public interface IFileHelper
    {
        bool Exists(string path);
        IEnumerable<string> ReadLines(string path);
    }
}