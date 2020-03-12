using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

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
            Cache = new Dictionary<string, IBinarySerializable>();
        }

        /// <summary>
        /// The file cache
        /// </summary>
        private static Dictionary<string, IBinarySerializable> Cache { get; }

        /// <summary>
        /// Reads a file or gets it from the cache
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="filePath">The file path to read from</param>
        /// <param name="settings">The game settings</param>
        /// <param name="mode">The file mode to use when reading</param>
        /// <returns>The file data</returns>
        public static T Read<T>(string filePath, GameSettings settings, FileMode mode = FileMode.None)
            where T : IBinarySerializable
        {
            // Check if it's been cached
            if (Cache.ContainsKey(filePath))
                return (T)Cache[filePath];

            // Open the file
            using (var fileStream = File.OpenRead(filePath))
            {
                // Helper method for getting the stream to use
                Stream GetStream()
                {
                    switch (mode)
                    {
                        case FileMode.None:
                            return fileStream;
                        
                        case FileMode.CompressedGZip:
                            // Create a memory stream to write to so we can get the position
                            var memStream = new MemoryStream();

                            // Decompress to the memory stream
                            using (var gZipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                                gZipStream.CopyTo(memStream);

                            // Set the position to the beginning
                            memStream.Position = 0;

                            return memStream;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    }
                }

                // Get the stream and use it when deserializing
                using (var stream = GetStream())
                {
                    // Create the deserializer
                    var deserializer = new BinaryDeserializer(stream, filePath, settings);

                    // Deserialize file
                    var fileData = deserializer.Read<T>();

                    // Make sure the entire file was read
                    if (stream.Position != stream.Length)
                        Debug.LogWarning("The entire file wasn't read");

                    // Cache the data
                    Cache[filePath] = fileData;

                    // Return the data
                    return fileData;
                }
            }
        }

        /// <summary>
        /// Writes the data from the cache to the specified path
        /// </summary>
        /// <param name="filePath">The file path to write to</param>
        /// <param name="settings">The game settings</param>
        public static void Write(string filePath, GameSettings settings)
        {
            // Get the cached data
            var data = Cache[filePath];

            // Create the file
            using (var file = File.Create(filePath))
            {
                // Create the serializer
                var serializer = new BinarySerializer(file, filePath, settings);

                // Serialize the file
                serializer.Write(data);
            }
        }

        /// <summary>
        /// The available serializable data types
        /// </summary>
        public static Type[] SerializableDataTypes { get; } =
        {
            typeof(PS1_R1_LevFile),
            typeof(PC_LevFile),
            typeof(PC_WorldFile),
            typeof(PC_Mapper_EventLocFile),
            typeof(PC_Mapper_EventManifestFile),
        };
    }

    /// <summary>
    /// The file mode to use when reading/writing
    /// </summary>
    public enum FileMode
    {
        /// <summary>
        /// Handle the file normally
        /// </summary>
        None,

        /// <summary>
        /// Handle the file as a GZip compressed file
        /// </summary>
        CompressedGZip
    }
}