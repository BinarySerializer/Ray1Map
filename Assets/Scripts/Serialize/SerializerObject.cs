using R1Engine.Serialize;
using System;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// A base binary serializer used for serializing/deserializing
    /// </summary>
    public abstract class SerializerObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseStream">The base stream</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="context">The serializer context, containing game settings</param>
        public SerializerObject(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// The serialize context, containing all the open files and the settings
        /// </summary>
        public Context Context { get; }
        public GameSettings GameSettings => Context.Settings;

        public abstract uint CurrentLength { get; }
        public abstract Pointer CurrentPointer { get; }

        public int Depth { get; protected set; } = 0;

        public abstract void Goto(Pointer offset);
        public void DoAt(Pointer offset, Action action) {
            if (offset != null) {
                Pointer off_current = CurrentPointer;
                Goto(offset);
                action();
                Goto(off_current);
            }
        }
        public T DoAt<T>(Pointer offset, Func<T> action) {
            if (offset != null) {
                Pointer off_current = CurrentPointer;
                Goto(offset);
                var result = action();
                Goto(off_current);
                return result;
            }

            return default;
        }

        public abstract void DoEncoded(IStreamEncoder encoder, Action action);

        /// <summary>
        /// Main Serialize method.
        /// </summary>
        /// <typeparam name="T">The type of value to serialize</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="name">A name can be provided optionally, for logging or text serialization purposes.</param>
        /// <returns>The object that was serialized</returns>
        public abstract T Serialize<T>(T obj, string name = null);

        public abstract T SerializeChecksum<T>(T calculatedChecksum, string name = null);

        /// <summary>
        /// Main Serialize method for seriazables.
        /// </summary>
        /// <typeparam name="T">The type of value to serialize</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="name">A name can be provided optionally, for logging or text serialization purposes.</param>
        /// <returns>The object that was serialized</returns>
        public abstract T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) where T : R1Serializable, new();

        public abstract Pointer SerializePointer(Pointer obj, Pointer anchor = null, string name = null);
        public abstract Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, string name = null) where T : R1Serializable, new();

        public abstract T[] SerializeArray<T>(T[] obj, decimal count, string name = null);
        public abstract T[] SerializeObjectArray<T>(T[] obj, decimal count, Action<T> onPreSerialize = null, string name = null) where T : R1Serializable, new();
        public abstract Pointer[] SerializePointerArray(Pointer[] obj, decimal count, Pointer anchor = null, string name = null);
        public abstract Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, decimal count, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, string name = null) where T : R1Serializable, new();

        public abstract string SerializeString(string obj, decimal? length = null, Encoding encoding = null, string name = null);
        public abstract T[] SerializeArraySize<T, U>(T[] obj, string name = null) where U : struct;

        public virtual T SerializeFile<T>(string relativePath, T obj, Action<T> onPreSerialize = null, string name = null) where T : R1Serializable, new() {
            T t = obj;
            DoAt(Context.FilePointer(relativePath), () => {
                t = SerializeObject<T>(obj, onPreSerialize: onPreSerialize, name: name);
            });
            return t;
            //return Context.FilePointer<T>(relativePath)?.Resolve(this, onPreSerialize: onPreSerialize).Value;
        }

        /// <summary>
        /// Begins calculating byte checksum for all following serialize operations
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to use</param>
        public virtual void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) {
        }

        /// <summary>
        /// Ends calculating the checksum and return the value
        /// </summary>
        /// <typeparam name="T">The type of checksum value</typeparam>
        /// <returns>The checksum value</returns>
        public virtual T EndCalculateChecksum<T>() {
            return default(T);
        }

        public virtual void BeginXOR(byte xorKey) { }
        public virtual void EndXOR() { }
    }
}