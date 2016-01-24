using System;
using System.Linq;
using System.Collections.Generic;

namespace TinyPng.Tools
{
    public sealed class ImageCompressorSettings
    {
        public IReadOnlyDictionary<String, FileEndpoints> ImageEndpoints { get; private set; }

        public Int32 MaxParallelJobs { get; private set; }

        public Int32 MaxFilesPerJob { get; private set; }

        public ImageCompressorSettings(
            String sourcePath,
            String targetPath,
            Int32 maxParallelJobs,
            Int32 maxFilesPerJob)
        {
            this.ImageEndpoints = DirectoryHelper
                .TraverseFiles(
                    sourcePath,
                    FileHelper.IsJpegOrPngFile)
                .Select(
                    p => new FileEndpoints(p, sourcePath, targetPath))
                .ToDictionary(
                    e => e.Source.FilePath, 
                    e => e);
            this.MaxParallelJobs = maxParallelJobs;
            this.MaxFilesPerJob = maxFilesPerJob;
        }

        public ImageCompressorSettings(
            IEnumerable<String> imageFiles,
            String sourcePath,
            String targetPath,
            Int32 maxParallelJobs,
            Int32 maxFilesPerJob)
        {
            if (imageFiles.Any(p => !FileHelper.IsJpegOrPngFile(p)))
            {
                throw new ArgumentException("imageFiles must contain only image path");
            }

            this.ImageEndpoints = imageFiles
                .Select(
                    p => new FileEndpoints(p, sourcePath, targetPath))
                .ToDictionary(
                    e => e.Source.FilePath,
                    e => e);
            this.MaxParallelJobs = maxParallelJobs;
            this.MaxFilesPerJob = maxFilesPerJob;
        }
    }
}
