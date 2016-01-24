using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TinyPng.Tools
{
    public static class DirectoryHelper 
    {
        private static readonly Object _syncRoot = new Object();

        public static DirectoryInfo GetOrCreateDirectory(String path)
        {
            lock (_syncRoot)
            {
                if (Directory.Exists(path))
                {
                    return new DirectoryInfo(path);
                }
                else
                {
                    return Directory.CreateDirectory(path);
                }
            }
        }

        private static String NormalizeDirectoryPath(String directory)
        {
            var path = Path.Combine(directory, "x");
            return path.Substring(0, path.Length - 1);
        }

        public static String GetRelativePath(String fullPath, String basePath)
        {
            var normalizedBasePath = NormalizeDirectoryPath(basePath);
            return fullPath.Remove(0, normalizedBasePath.Length);
        }

        public static IEnumerable<String> TraverseFiles(String path, Func<String, Boolean> predicate)
        {
            foreach (var file in Directory.EnumerateFiles(path).Where(predicate))
            {
                yield return file;
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                foreach (var file in TraverseFiles(dir, predicate))
                {
                    yield return file;
                }
            }
        }
    }
}

