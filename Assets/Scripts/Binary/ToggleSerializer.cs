using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// A serializer which toggles between reading and writing depending on the name
    /// </summary>
    public class ToggleSerializer : SerializerObject
    {
        public ToggleSerializer(Context context, Func<string, bool> shouldWriteFunc, Pointer baseOffset) : base(context) {
            // Set properties
            ShouldWriteFunc = shouldWriteFunc;
            Serializer = context.Serializer;
            Deserializer = context.Deserializer;
            CurrentName = new Stack<string>();

            // Init serializers
            Serializer.Goto(baseOffset);
            Deserializer.Goto(baseOffset);
            CurrentSerializer = Deserializer;
        }

        protected SerializerObject Serializer { get; }
        protected SerializerObject Deserializer { get; }
        protected SerializerObject CurrentSerializer { get; set; }
        protected Func<string, bool> ShouldWriteFunc { get; }
        protected void UpdateCurrentSerializer(string name = null) {
            var fullName = String.Join(".", CurrentName.Append(name).Where(x => !String.IsNullOrWhiteSpace(x)));
            var s = ShouldWriteFunc(fullName) ? Serializer : Deserializer;
            if (CurrentSerializer != s) {
                SystemLog?.LogInfo("{0} - {1}", fullName, s);
                SwitchSerializer(s);
            }
        }
        protected void SwitchSerializer(SerializerObject s) {
            if (s != CurrentSerializer) {
                s.Goto(CurrentSerializer.CurrentPointer);
                CurrentSerializer = s;
            }
        }

        protected Stack<string> CurrentName { get; }

        public override long CurrentLength => CurrentSerializer.CurrentLength;

        public override bool HasCurrentPointer => CurrentSerializer.HasCurrentPointer;

        /// <summary>
        /// The current binary file being used by the serializer
        /// </summary>
        public override BinaryFile CurrentBinaryFile => CurrentSerializer.CurrentBinaryFile;

        public override long CurrentFileOffset => CurrentSerializer.CurrentFileOffset;

        public override SerializerDefaults Defaults {
            get => CurrentSerializer.Defaults;
            set {
                Deserializer.Defaults = value;
                Serializer.Defaults = value;
            }
        }

        public override void Goto(Pointer offset) => CurrentSerializer.Goto(offset);
        public override void Align(int alignBytes = 4, Pointer baseOffset = null, bool? logIfNotNull = null)
        {
            CurrentSerializer.Align(alignBytes, baseOffset, logIfNotNull);
        }

        public override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, bool allowLocalPointers = false, string filename = null) {
            SwitchSerializer(Deserializer);
            CurrentSerializer.DoEncoded(encoder, action, endianness: endianness, allowLocalPointers: allowLocalPointers, filename: filename);
        }
        public override Pointer BeginEncoded(IStreamEncoder encoder, Endian? endianness = null, bool allowLocalPointers = false, string filename = null) {
            throw new NotSupportedException("BeginEncoded and EndEncoded are not supported in ToggleSerializer");
        }
        public override void EndEncoded(Pointer endPointer) {
            throw new NotSupportedException("BeginEncoded and EndEncoded are not supported in ToggleSerializer");
        }

        public override void DoEndian(Endian endianness, Action action) {
            CurrentSerializer.DoEndian(endianness, action);
        }
        public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) {
            SwitchSerializer(Deserializer);
            return CurrentSerializer.SerializeChecksum(calculatedChecksum, name);
        }
        public override Pointer SerializePointer(Pointer obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializePointer(obj, size, anchor, allowInvalid, nullValue, name);
        }
        public override Pointer<T> SerializePointer<T>(Pointer<T> obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializePointer(obj, size, anchor, allowInvalid, nullValue, name);
        }

        public override T Serialize<T>(T obj, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.Serialize(obj, name);
        }

        public override T? SerializeNullable<T>(T? obj, string name = null)
        {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializeNullable(obj, name);
        }

        public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) {

            try
            {
                Depth++;
                CurrentName.Push(name);
                obj ??= new T();
                if (obj.Context == null || obj.Context != Context)
                {
                    // reinitialize object
                    obj.Init(CurrentPointer);
                }
                onPreSerialize?.Invoke(obj);
                obj.Serialize(this);
            }
            finally
            {
                Depth--;
                CurrentName.Pop();
            }
            return obj;
        }
        public override T[] SerializeArray<T>(T[] obj, long count, string name = null) {
            if (obj == null) obj = new T[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = Serialize<T>(obj[i], name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override T?[] SerializeNullableArray<T>(T?[] obj, long count, string name = null)
        {
            if (obj == null) obj = new T?[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializeNullable<T>(obj[i], name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T, int> onPreSerialize = null, string name = null) {
            if (obj == null) obj = new T[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializeObject<T>(
                    obj: obj[i], 
                    onPreSerialize: onPreSerialize == null ? (Action<T>)null : x => onPreSerialize(x, i), 
                    name: name == null ? null : $"{name}[{i}]");

            return obj;
        }
        public override T[] SerializeArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null, string name = null)
        {
            for (int i = 0; i < obj.Length; i++)
                obj[i] = Serialize<T>(obj[i], name: name == null ? null : $"{name}[{i}]");

            return obj;
        }

        public override T?[] SerializeNullableArrayUntil<T>(T?[] obj, Func<T?, bool> conditionCheckFunc, Func<T?> getLastObjFunc = null,
            string name = null)
        {
            for (int i = 0; i < obj.Length; i++)
                obj[i] = SerializeNullable<T>(obj[i], name: name == null ? null : $"{name}[{i}]");

            return obj;
        }

        public override T[] SerializeObjectArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null,
            Action<T, int> onPreSerialize = null, string name = null)
        {
            for (int i = 0; i < obj.Length; i++)
                obj[i] = SerializeObject<T>(
                    obj: obj[i], 
                    onPreSerialize: onPreSerialize == null ? (Action<T>)null : x => onPreSerialize(x, i), 
                    name: name == null ? null : $"{name}[{i}]");

            return obj;
        }

        public override Pointer[] SerializePointerArrayUntil(Pointer[] obj, Func<Pointer, bool> conditionCheckFunc, Func<Pointer> getLastObjFunc = null,
            PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null,
            string name = null)
        {
            throw new NotImplementedException();
        }

        public override Pointer[] SerializePointerArray(Pointer[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null) {
            if (obj == null) obj = new Pointer[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializePointer(obj[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }
        public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null) {
            if (obj == null) obj = new Pointer<T>[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializePointer<T>(
                    obj: obj[i], 
                    size: size, 
                    anchor: anchor, 
                    allowInvalid: allowInvalid, 
                    nullValue: nullValue, 
                    name: name == null ? null : $"{name}[{i}]");

            return obj;
        }

        public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializeString(obj, length, encoding, name);
        }

        public override string[] SerializeStringArray(string[] obj, long count, long? length = null, Encoding encoding = null, string name = null) {
            if (obj == null) obj = new string[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializeString(obj[i], length: length, encoding: encoding, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) {
            SwitchSerializer(Deserializer);
            return CurrentSerializer.SerializeArraySize<T, U>(obj, name);
        }

        public override void Log(string logString, params object[] args) => CurrentSerializer.Log(logString, args);

        public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
        {
            SwitchSerializer(Deserializer);
            CurrentSerializer.SerializeBitValues(serializeFunc);
        }

        public override void DoBits<T>(Action<BitSerializerObject> serializeFunc) {
            SwitchSerializer(Deserializer);
            CurrentSerializer.DoBits<T>(serializeFunc);
        }

        public override bool FullSerialize => CurrentSerializer.FullSerialize;
    }
}