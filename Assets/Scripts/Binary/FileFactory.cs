using R1Engine;
using System.Collections.Generic;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Handles cached files
    /// </summary>
    public static class FileFactory
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        static FileFactory()
        {
            Cache = new Dictionary<string, ISerializableFile>();
        }

        /// <summary>
        /// The file cache
        /// </summary>
        private static Dictionary<string, ISerializableFile> Cache { get; }

        /// <summary>
        /// Reads a file or gets it from the cache
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="filePath">The file path to read from</param>
        /// <returns>The file data</returns>
        public static T Read<T>(string filePath)
            where T : ISerializableFile
        {
            // Check if it's been cached
            if (Cache.ContainsKey(filePath))
                return (T)Cache[filePath];

            // Open the file
            using (var file = File.OpenRead(filePath))
            {
                // Deserialize file
                var fileData = file.Read<T>();

                // Cache the data
                Cache[filePath] = fileData;

                // Return the data
                return fileData;
            }
        }

        /// <summary>
        /// Writes the data from the cache to the specified path
        /// </summary>
        /// <param name="filePath">The file path to write to</param>
        public static void Write(string filePath)
        {
            // Get the cached data
            var data = Cache[filePath];

            // Open the file
            using (var file = File.OpenWrite(filePath))
            {
                // Serialize the file
                file.Write(data);

                // Set the file length to the current position
                file.SetLength(file.Position);
            }
        }
    }
}