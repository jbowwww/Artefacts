using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemArtefacts
{
    public class Directory : FileSystemEntry
    {
        public DirectoryInfo DirectoryInfo
        {
            get { return (DirectoryInfo)base.Info; }
            protected set { base.Info = value; }
        }

        public Directory(string path) : base(path)
        {
            path.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            base.Info = new DirectoryInfo(path);
        }
    }
}
