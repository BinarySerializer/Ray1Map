using R1Engine.Serialize;
using System;

namespace R1Engine
{
    /// <summary>
    /// Handles reading and writing serializable files
    /// </summary>
    public static class FileFactory
    {
        /// <summary>
        /// Reads a file or gets it from the cache
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="filePath">The file path to read from</param>
        /// <param name="context">The context</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        /// <returns>The file data</returns>
        public static T Read<T>(string filePath, Context context, Action<SerializerObject, T> onPreSerialize = null)
            where T : R1Serializable, new()
        {
            // Try cached version, to avoid creating the deserializer unless necessary
            T mainObj = context.GetMainFileObject<T>(filePath);

            if (mainObj != null) 
                return mainObj;

            // Use deserializer
            return context.Deserializer.SerializeFile<T>(filePath, null, onPreSerialize: (t) => onPreSerialize?.Invoke(context.Deserializer, t), name: filePath);
        }

        /// <summary>
        /// Reads a file from an offset or gets it from the cache
        /// </summary>
        /// <typeparam name="T">The type to serialize to</typeparam>
        /// <param name="offset">The offset to read from</param>
        /// <param name="context">The context</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        /// <param name="name">Optional name for logging</param>
        /// <returns>The file data</returns>
        public static T Read<T>(Pointer offset, Context context, Action<SerializerObject, T> onPreSerialize = null, string name = null)
            where T : R1Serializable, new() 
        {
            // Try cached version, to avoid creating the deserializer unless necessary
            T mainObj = default;
        
            // Use deserializer
            context.Deserializer.DoAt(offset, () => {
               mainObj = context.Deserializer.SerializeObject<T>(mainObj, onPreSerialize: (t) => onPreSerialize?.Invoke(context.Deserializer, t), name: name);
            });
            
            return mainObj;
        }

        // TODO: Improve this system
        public static T ReadMapper<T>(string filePath, Context context, Action<T> onPreSerialize = null)
            where T : MapperTextSerializable, new() 
        {
            // Create the data object
            T t = new T();

            // Option pre-serialize action
            onPreSerialize?.Invoke(t);

            // Serialize the object
            t.Serialize(filePath, context, true);
            
            // Return the object
            return t;
        }

        /// <summary>
        /// Writes the data from the cache to the specified path
        /// </summary>
        /// <param name="filePath">The file path to write to</param>
        /// <param name="context">The context</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        public static T Write<T>(string filePath, Context context, Action<SerializerObject, T> onPreSerialize = null) 
            where T : R1Serializable, new() 
        {
            return context.FilePointer<T>(filePath)?.Resolve(context.Serializer, onPreSerialize: (t) => onPreSerialize?.Invoke(context.Serializer, t)).Value;
        }

        /// <summary>
        /// Writes the data to the specified path
        /// </summary>
        /// <param name="filePath">The file path to write to</param>
        /// <param name="obj">The object to write</param>
        /// <param name="context">The context</param>
        /// <param name="onPreSerialize">Optional action to run before serializing</param>
        public static T Write<T>(string filePath, T obj, Context context, Action<SerializerObject, T> onPreSerialize = null) 
            where T : R1Serializable, new() 
        {
            return context.Serializer.SerializeFile<T>(filePath, obj, onPreSerialize: (t) => onPreSerialize?.Invoke(context.Serializer, t), name: filePath);
        }

        public static T WriteMapper<T>(string filePath, T obj, Context context, Action<T> onPreSerialize = null)
            where T : MapperTextSerializable, new()
        {
            // Option pre-serialize action
            onPreSerialize?.Invoke(obj);

            // Serialize the object
            obj.Serialize(filePath, context, false);

            // Return the object
            return obj;
        }
    }
}