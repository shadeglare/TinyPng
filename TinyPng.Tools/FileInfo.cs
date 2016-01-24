using System;
using System.IO;

namespace TinyPng.Tools
{
    public sealed class FileInfo {

        public String FileName { get; private set; }

        public String DirectoryName { get; private set; }

        public String FilePath { get { return Path.Combine(this.DirectoryName, this.FileName); } }

        public FileInfo(String fileName, String directoryName) 
        {
            this.FileName = fileName;
            this.DirectoryName = directoryName;
        }

        public FileInfo(String filePath)
        {
            this.FileName = Path.GetFileName(filePath);
            this.DirectoryName = Path.GetDirectoryName(filePath);
        }
    }
}

