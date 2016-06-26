using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemArtefacts
{
    public class File : FileSystemEntry
    {
        public FileInfo DirectoryInfo
        {
            get { return (FileInfo)base.Info; }
            protected set { base.Info = value; }
        }

        public File(string path) : base(path)
        {
            base.Info = new FileInfo(path);
        }
    }
}
