using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemArtefacts
{
    public class FileSystemEntry
    {
        public string Path { get; private set; }
        public FileSystemInfo Info { get; protected set; }

        public FileSystemEntry(string path)
        {
            Path = path;
        }
    }
}
