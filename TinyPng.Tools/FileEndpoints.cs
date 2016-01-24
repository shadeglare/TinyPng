using System;
using System.IO;

namespace TinyPng.Tools
{
    public sealed class FileEndpoints
    {
        public FileInfo Source { get; private set; }

        public FileInfo Target { get; private set; }

        public FileEndpoints(FileInfo source, FileInfo target)
        {
            this.Source = source;
            this.Target = target;
        }

        public FileEndpoints(
            String sourceFilePath,
            String sourceBasePath,
            String targetBasePath)
        {
            var targetRelativePath = DirectoryHelper.GetRelativePath(sourceFilePath, sourceBasePath);
            var targetFilePath = Path.Combine(targetBasePath, targetRelativePath);

            this.Source = new FileInfo(sourceFilePath);
            this.Target = new FileInfo(targetFilePath);
        }
    }
}

