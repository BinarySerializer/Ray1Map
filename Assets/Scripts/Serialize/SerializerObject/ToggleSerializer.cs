using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// A serializer which toggles between reading and writing depending on the name
    /// </summary>
    public class ToggleSerializer : SerializerObject
    {
        public ToggleSerializer(Context context, Func<string, bool> shouldWriteFunc, Pointer baseOffset) : base(context)
        {
            // Set properties
            ShouldWriteFunc = shouldWriteFunc;
            Serializer = context.Serializer;
            Deserializer = context.Deserializer;
            CurrentName = new List<string>();

            // Init serializers
            Serializer.Goto(baseOffset);
            Deserializer.Goto(baseOffset);
        }

        protected SerializerObject Serializer { get; }
        protected SerializerObject Deserializer { get; }
        protected Func<string, bool> ShouldWriteFunc { get; }
        protected SerializerObject GetCurrentSerializer(string name = null)
        {
            var fullName = String.Join(".", CurrentName.Append(name).Where(x => !String.IsNullOrWhiteSpace(x)));
            var s = ShouldWriteFunc(fullName) ? Serializer : Deserializer;
            return s;
        }
        protected List<string> CurrentName { get; }

        public override uint CurrentLength => GetCurrentSerializer().CurrentLength;
        public override Pointer CurrentPointer => GetCurrentSerializer().CurrentPointer;
        public override void Goto(Pointer offset) => GetCurrentSerializer().Goto(offset);
        public override void DoEncoded(IStreamEncoder encoder, Action action) => GetCurrentSerializer().DoEncoded(encoder, action);
        public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) => GetCurrentSerializer(name).SerializeChecksum(calculatedChecksum, name);
        public override Pointer SerializePointer(Pointer obj, Pointer anchor = null, bool allowInvalid = false, string name = null) => GetCurrentSerializer(name).SerializePointer(obj, anchor, allowInvalid, name);
        public override Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null,
            bool allowInvalid = false, string name = null) => GetCurrentSerializer(name).SerializePointer(obj, anchor, resolve, onPreSerialize, allowInvalid, name);
        public override T Serialize<T>(T obj, string name = null) => GetCurrentSerializer(name).Serialize(obj, name);
        public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
        {
            CurrentName.Add(name);
            var output = GetCurrentSerializer(name).SerializeObject(obj, onPreSerialize, name);
            CurrentName.RemoveAt(CurrentName.Count - 1);
            return output;
        }
        public override T[] SerializeArray<T>(T[] obj, long count, string name = null) => GetCurrentSerializer(name).SerializeArray(obj, count, name);
        public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null) => GetCurrentSerializer(name).SerializeObjectArray<T>(obj, count, onPreSerialize, name);
        public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false,
            string name = null) => GetCurrentSerializer(name).SerializePointerArray(obj, count, anchor, allowInvalid, name);
        public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false,
            Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) => GetCurrentSerializer(name).SerializePointerArray(obj, count, anchor, resolve, onPreSerialize, allowInvalid, name);

        public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null) => GetCurrentSerializer(name).SerializeString(obj, length, encoding, name);

        public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null) => GetCurrentSerializer(name).SerializeStringArray(obj, count, length, encoding, name);

        public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) => GetCurrentSerializer(name).SerializeArraySize<T, U>(obj, name);

        public override void Log(string logString) => GetCurrentSerializer().Log(logString);

        public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) => GetCurrentSerializer().SerializeBitValues<T>(serializeFunc);

        public override bool FullSerialize => GetCurrentSerializer().FullSerialize;
    }
}