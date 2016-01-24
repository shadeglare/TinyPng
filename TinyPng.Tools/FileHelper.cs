using System;

namespace TinyPng.Tools
{
    public static class FileHelper
    {
        public static Boolean IsJpegOrPngFile(String filePath)
        {
            return filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg");
        }
    }
}

