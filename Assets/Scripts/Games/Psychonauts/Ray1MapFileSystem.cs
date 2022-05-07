using System;
using System.Collections.Generic;
using System.IO;
using PsychoPortal.Unity;

namespace Ray1Map.Psychonauts
{
    public class Ray1MapFileSystem : IFileSystemService
    {
        public bool FileExists(string filePath)
        {
            return FileSystem.FileExists(filePath);
        }

        public bool DirectoryExists(string dirPath)
        {
            return FileSystem.DirectoryExists(dirPath);
        }

        public IEnumerable<string> EnumerateFiles(string dirPath, string searchPattern, SearchOption searchOption)
        {
            return FileSystem.mode == FileSystem.Mode.Normal
                ? Directory.EnumerateFiles(dirPath, searchPattern, searchOption)
                : throw new NotSupportedException("Enumerating files on web is not supported");
        }

        public Stream OpenFile(string filePath, FileAccess access)
        {
            if (access == FileAccess.Read)
                return FileSystem.GetFileReadStream(filePath);

            return FileSystem.mode == FileSystem.Mode.Normal
                ? File.Open(filePath, FileMode.OpenOrCreate, access, FileShare.None)
                : throw new NotSupportedException("Opening a file for writing on web is not supported");
        }
    }
}