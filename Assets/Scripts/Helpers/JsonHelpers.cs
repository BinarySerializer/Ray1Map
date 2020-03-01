using System;
using System.IO;
using Newtonsoft.Json;

namespace R1Engine
{
    /// <summary>
    /// Helper methods for JSON
    /// </summary>
    public static class JsonHelpers
    {
        /// <summary>
        /// Serializes an object to a file
        /// </summary>
        /// <typeparam name="T">The type of file to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="filePath">The file to serialize to</param>
        public static void SerializeToFile<T>(T obj, string filePath)
        {
            // Serialize to JSON
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, new ByteArrayHexConverter());

            // Write to output
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Deserializes an object from a file
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="filePath">The file to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromFile<T>(string filePath)
        {
            // Read the JSON
            var json = File.ReadAllText(filePath);

            // Return the deserialized object
            return JsonConvert.DeserializeObject<T>(json, new ByteArrayHexConverter());
        }

        /// <summary>
        /// Deserializes an object from a file
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="filePath">The file to deserialize</param>
        /// <param name="type">The type of object to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromFile<T>(string filePath, Type type)
        {
            // Read the JSON
            var json = File.ReadAllText(filePath);

            // Return the deserialized object
            return (T)JsonConvert.DeserializeObject(json, type, new ByteArrayHexConverter());
        }
    }
}