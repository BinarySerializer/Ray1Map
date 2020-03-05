using System;
using System.Collections.Generic;
using System.IO;
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
        /// <returns>The file data</returns>
        public static T Read<T>(string filePath, GameSettings settings)
            where T : IBinarySerializable
        {
            // Check if it's been cached
            if (Cache.ContainsKey(filePath))
                return (T)Cache[filePath];

            // Open the file
            using (var file = File.OpenRead(filePath))
            {
                // Create the deserializer
                var deserializer = new BinaryDeserializer(file, filePath, settings);

                // Deserialize file
                var fileData = deserializer.Read<T>();

                // Make sure the entire file was read
                if (file.Position != file.Length)
                    Debug.LogError("The entire file wasn't read");

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
            typeof(PC_RD_EventLocFile),
            typeof(PC_RD_EventManifestFile),
        };
    }
}