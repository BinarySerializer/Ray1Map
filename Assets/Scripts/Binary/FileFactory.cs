using R1Engine.Serialize;
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
        }

        /// <summary>
        /// Reads a file or gets it from the cache
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="filePath">The file path to read from</param>
        /// <param name="settings">The game settings</param>
        /// <param name="mode">The file mode to use when reading</param>
        /// <returns>The file data</returns>
        public static T Read<T>(string filePath, Context context, Action<T> onPreSerialize = null)
            where T : R1Serializable, new()
        {
            // Try cached version, to avoid creating the deserializer unless necessary
            T t = context.GetMainFileObject<T>(filePath);
            if (t != null) return t;
            // Use deserializer
            return context.Deserializer.SerializeFile(filePath, onPreSerialize: onPreSerialize, name: filePath);
        }

        // TODO: Improve this system
        public static T ReadMapper<T>(string filePath, Context context, Action<T> onPreSerialize = null)
            where T : MapperEngineSerializable, new() {
            T t = new T();
            onPreSerialize?.Invoke(t);
            t.Serialize(filePath, context, SerializerMode.Read);
            return t;
        }

        /// <summary>
        /// Writes the data from the cache to the specified path
        /// </summary>
        /// <param name="filePath">The file path to write to</param>
        /// <param name="settings">The game settings</param>
        public static T Write<T>(string filePath, Context context, Action<T> onPreSerialize = null) where T : R1Serializable, new() {
            return context.FilePointer<T>(filePath)?.Resolve(context.Serializer, onPreSerialize: onPreSerialize).Value;
        }
    }
}