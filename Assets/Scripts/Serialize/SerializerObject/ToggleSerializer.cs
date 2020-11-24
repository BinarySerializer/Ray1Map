using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A serializer which toggles between reading and writing depending on the name
    /// </summary>
    public class ToggleSerializer : SerializerObject {
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
                Debug.Log(fullName + " - " + s);
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

        public override uint CurrentLength => CurrentSerializer.CurrentLength;
        public override Pointer CurrentPointer => CurrentSerializer.CurrentPointer;
        public override void Goto(Pointer offset) => CurrentSerializer.Goto(offset);
        public override void DoEncoded(IStreamEncoder encoder, Action action, BinaryFile.Endian? endianness = null) {
            SwitchSerializer(Deserializer);
            CurrentSerializer.DoEncoded(encoder, action, endianness);
        }
        public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) {
            SwitchSerializer(Deserializer);
            return CurrentSerializer.SerializeChecksum(calculatedChecksum, name);
        }
        public override Pointer SerializePointer(Pointer obj, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializePointer(obj, anchor, allowInvalid, name);
        }
        public override Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializePointer(obj, anchor, resolve, onPreSerialize, allowInvalid, name);
        }
        public override T Serialize<T>(T obj, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.Serialize(obj, name);
        }
        public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) {

            Depth++;
            CurrentName.Push(name);
            if (obj == null) obj = new T();
            onPreSerialize?.Invoke(obj);
            if (obj.Context == null || obj.Context != Context) {
                // reinitialize object
                obj.Init(CurrentPointer);
            }
            obj.Serialize(this);
            Depth--;
            CurrentName.Pop();
            return obj;
        }
        public override T[] SerializeArray<T>(T[] obj, long count, string name = null) {
            if (obj == null) obj = new T[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = Serialize<T>(obj[i], name: name == null ? null : name + "[" + i + "]");

            return obj;
        }
        public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null) {
            if (obj == null) obj = new T[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializeObject<T>(obj[i], onPreSerialize: onPreSerialize, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }
        public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            if (obj == null) obj = new Pointer[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializePointer(obj[i], anchor: anchor, allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }
        public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            if (obj == null) obj = new Pointer<T>[count];
            else if (count != obj.Length) Array.Resize(ref obj, (int)count);

            for (int i = 0; i < count; i++)
                obj[i] = SerializePointer<T>(obj[i], anchor: anchor, resolve: resolve, onPreSerialize: onPreSerialize, allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null) {
            UpdateCurrentSerializer(name);
            return CurrentSerializer.SerializeString(obj, length, encoding, name);
        }

        public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null) {
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

        public override void Log(string logString) => CurrentSerializer.Log(logString);

        public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) {
            SwitchSerializer(Deserializer);
            CurrentSerializer.SerializeBitValues<T>(serializeFunc);
        }

        public override bool FullSerialize => CurrentSerializer.FullSerialize;
    }
}