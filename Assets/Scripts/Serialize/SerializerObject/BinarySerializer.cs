using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// A binary serializer used for serializing
    /// </summary>
    public class BinarySerializer : SerializerObject, IDisposable {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseStream">The base stream</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="settings">The game settings</param>
        public BinarySerializer(Context context) : base(context)
        { }

        protected Writer writer;
        protected BinaryFile currentFile;
        protected Dictionary<BinaryFile, Writer> writers = new Dictionary<BinaryFile, Writer>();
        private string LogPrefix => Settings.Log ? ("(WRITE) " + CurrentPointer + ":" + new string(' ', (Depth + 1) * 2)) : null;

        public override Pointer CurrentPointer {
            get {
                if (currentFile == null) {
                    return null;
                }
                uint curPos = (uint)writer.BaseStream.Position;
                return new Pointer((uint)(curPos + currentFile.baseAddress), currentFile);
            }
        }

        public override uint CurrentLength => (uint)writer.BaseStream.Length; // can be modified!

        protected void SwitchToFile(BinaryFile newFile) {
            if (newFile == null) return;
            if (!writers.ContainsKey(newFile)) {
                writers.Add(newFile, newFile.CreateWriter());
            }
            writer = writers[newFile];
            currentFile = newFile;
        }

        /// <summary>
        /// Writes a supported value to the stream
        /// </summary>
        /// <param name="value">The value</param>
        protected void Write(object value)
        {
            if (value is byte[] ba)
                writer.Write(ba);

            else if (value is Array a)
                foreach (var item in a)
                    Write(item);
            else if (value.GetType().IsEnum) {
                Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
            } else if (value is bool bo)
                writer.Write((byte)(bo ? 1 : 0));

            else if (value is sbyte sb)
                writer.Write((byte)sb);

            else if (value is byte by)
                writer.Write(by);

            else if (value is short sh)
                writer.Write(sh);

            else if (value is ushort ush)
                writer.Write(ush);

            else if (value is int i32)
                writer.Write(i32);

            else if (value is uint ui32)
                writer.Write(ui32);

            else if (value is long lo)
                writer.Write(lo);

            else if (value is ulong ulo)
                writer.Write(ulo);

            else if (value is float fl)
                writer.Write(fl);

            else if (value is double dou)
                writer.Write(dou);

            else if (value is string s)
                writer.WriteNullDelimitedString(s);

            else if (value is null)
                throw new ArgumentNullException(nameof(value));

            else
                throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported.");
        }

        /// <summary>
        /// Writes a null-terminated string to the stream
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(string) " + (name ?? "<no name>") + ": " + obj);
            }
            if (length.HasValue) {
                writer.WriteString(obj, length.Value, encoding: encoding);
            } else {
                writer.WriteNullDelimitedString(obj, encoding: encoding);
            }
            return obj;
        }

        public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
        {
            if (Settings.Log)
                Context.Log.Log(LogPrefix + "(String[" + count + "]) " + (name ?? "<no name>"));

            for (int i = 0; i < count; i++)
                // Read the value
                SerializeString(obj[i], length, encoding, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        /// <summary>
        /// Begins calculating byte checksum for all decrypted bytes read from the stream
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to use</param>
        public override void BeginCalculateChecksum(IChecksumCalculator checksumCalculator) {
            writer.BeginCalculateChecksum(checksumCalculator);
        }

        /// <summary>
        /// Ends calculating the checksum and return the value
        /// </summary>
        /// <typeparam name="T">The type of checksum value</typeparam>
        /// <returns>The checksum value</returns>
        public override T EndCalculateChecksum<T>() {
            return writer.EndCalculateChecksum<T>();
        }

        public override void BeginXOR(byte xorKey) {
            writer.BeginXOR(xorKey);
        }
        public override void EndXOR() {
            writer.EndXOR();
        }

        public override void DoXOR(byte xorKey, Action action)
        {
            var prevKey = writer.GetXOR();
            BeginXOR(xorKey);
            action();

            if (prevKey == null)
                EndXOR();
            else
                BeginXOR(prevKey.Value);
        }

        public override void Goto(Pointer offset) {
            if (offset == null) return;
            if (offset.file != currentFile) {
                SwitchToFile(offset.file);
            }
            writer.BaseStream.Position = offset.FileOffset;
        }

        public override T Serialize<T>(T obj, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(" + typeof(T) + ") " + (name ?? "<no name>") + ": " + obj.ToString());
            }
            Write(obj);
            return obj;
        }

        public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(" + typeof(T) + ") " + (name ?? "<no name>") + ": " + calculatedChecksum.ToString());
            }
            Write(calculatedChecksum);
            return calculatedChecksum;
        }

        public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Object: " + typeof(T) + ") " + (name ?? "<no name>"));
            }
            Depth++;
            onPreSerialize?.Invoke(obj);
            if (obj.Context == null || obj.Context != Context) {
                // reinitialize object
                obj.Init(CurrentPointer);
            }
            obj.Serialize(this);
            Depth--;
            return obj;
        }

        public override Pointer SerializePointer(Pointer obj, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Pointer): " + obj?.ToString());
            }
            if (obj == null) {
                Write((uint)0);
            } else {
                Write(obj.SerializedOffset);
            }
            return obj;
        }

        public override Pointer<T> SerializePointer<T>(Pointer<T> obj, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Pointer<T>: " + typeof(T) + ") " + (name ?? "<no name>"));
            }
            Depth++;
            if (obj == null || obj.pointer == null) {
                Serialize<uint>(0, name: "Pointer");
            } else {
                Serialize<uint>(obj.pointer.SerializedOffset, name: "Pointer");
                if (resolve && obj.Value != null) {
                    DoAt(obj.pointer, () => {
                        SerializeObject<T>(obj.Value, onPreSerialize: onPreSerialize, name: "Value");
                    });
                }
            }
            Depth--;
            return obj;
        }

        public override T[] SerializeArray<T>(T[] obj, long count, string name = null) {
            if (Settings.Log) {
                if (typeof(T) == typeof(byte)) {
                    string normalLog = LogPrefix + "(" + typeof(T) + "[" + count + "]) " + (name ?? "<no name>") + ": ";
                    Context.Log.Log(normalLog
                        + Util.ByteArrayToHexString((byte[])(object)obj, Align: 16, NewLinePrefix: new string(' ', normalLog.Length), MaxLines: 10));
                } else {
                    Context.Log.Log(LogPrefix + "(" + typeof(T) + "[" + count + "]) " + (name ?? "<no name>"));
                }
            }
            // Use byte writing method if requested
            if (typeof(T) == typeof(byte)) {
                writer.Write((byte[])(object)obj);
                return obj;
            }

            for (int i = 0; i < count; i++)
                // Read the value
                Serialize<T>(obj[i], name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T> onPreSerialize = null, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Object[] " + typeof(T) + "[" + count + "]) " + (name ?? "<no name>"));
            }
            for (int i = 0; i < count; i++)
                // Read the value
                SerializeObject<T>(obj[i], onPreSerialize: onPreSerialize, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override Pointer[] SerializePointerArray(Pointer[] obj, long count, Pointer anchor = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Pointer[" + count + "]) " + (name ?? "<no name>"));
            }
            for (int i = 0; i < count; i++)
                // Read the value
                SerializePointer(obj[i], allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, Pointer anchor = null, bool resolve = false, Action<T> onPreSerialize = null, bool allowInvalid = false, string name = null) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + "(Pointer<" + typeof(T) + ">[" + count + "]) " + (name ?? "<no name>"));
            }
            for (int i = 0; i < count; i++)
                // Read the value
                SerializePointer<T>(obj[i], resolve: resolve, onPreSerialize: onPreSerialize, allowInvalid: allowInvalid, name: name == null ? null : name + "[" + i + "]");

            return obj;
        }

        public override T[] SerializeArraySize<T, U>(T[] obj, string name = null) {
            U Size = (U)Convert.ChangeType((obj?.Length) ?? 0, typeof(U));
            //U Size = (U)(object)((obj?.Length) ?? 0);
            Size = Serialize<U>(Size, name: name +".Length");
            return obj;

            /*if (Eta == null) {
                // Read the ETA data into a 3-fold array
                Eta = new PC_Eta[Size][][];
            }*/
        }

        public override void SerializeBitValues<T>(Action<SerializeBits> serializeFunc) {
            int valueInt = 0;
            int pos = 0;

            // Set bits
            serializeFunc((v, length, name) => {
                valueInt = BitHelpers.SetBits(valueInt, v, length, pos);
                Log($"  ({typeof(T)}) {name ?? "<no name>"}: {v}");
                pos += length;
                return v;
            });

            // Serialize value
            Serialize<T>((T)Convert.ChangeType(valueInt, typeof(T)), name: "Value");
        }

        public void Dispose() {
            foreach (KeyValuePair <BinaryFile, Writer> w in writers) {
                w.Key.EndWrite(w.Value);
            }
            writers.Clear();
            writer = null;
        }

        public void DisposeFile(BinaryFile file) {
            if (writers.ContainsKey(file)) {
                Writer w = writers[file];
                file.EndWrite(w);
                writers.Remove(file);
            }
        }
        public override void DoEncoded(IStreamEncoder encoder, Action action) {
            // Encode the data into a stream
            Stream encoded = null;
            using(MemoryStream memStream = new MemoryStream()) {
                // Stream key
                string key = CurrentPointer.ToString() + "_decoded";

                // Add the stream
                StreamFile sf = new StreamFile(key, memStream, Context)
                {
                    Endianness = currentFile.Endianness
                };
                Context.AddFile(sf);

                DoAt(sf.StartPointer, () => {
                    action();
                    memStream.Position = 0;
                    encoded = encoder.EncodeStream(memStream);
                });

                Context.RemoveFile(sf);
            }
            // Turn stream into array & write bytes
            if (encoded != null) {
                using (MemoryStream ms = new MemoryStream()) {
                    encoded.CopyTo(ms);
                    writer.Write(ms.ToArray());
                }
                encoded.Close();
            }
        }

        public override void Log(string logString) {
            if (Settings.Log) {
                Context.Log.Log(LogPrefix + logString);
            }
        }
	}
}