using System;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// A binary serializer used for serializing
    /// </summary>
    public class BinarySerializer : BaseBinarySerializer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseStream">The base stream</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="settings">The game settings</param>
        public BinarySerializer(Stream baseStream, string filePath, GameSettings settings) : base(baseStream, filePath, settings)
        { }

        /// <summary>
        /// Writes a supported value to the stream
        /// </summary>
        /// <param name="value">The value</param>
        public void Write(object value)
        {
            if (value is IBinarySerializable serializable)
                serializable.Serialize(this);

            else if (value is byte[] ba)
                BaseStream.Write(ba, 0, ba.Length);

            else if (value is Array a)
                foreach (var item in a)
                    Write(item);

            else if (value is bool bo)
                BaseStream.WriteByte((byte)(bo ? 1 : 0));

            else if (value is sbyte sb)
                BaseStream.WriteByte((byte)sb);

            else if (value is byte by)
                BaseStream.WriteByte(by);

            else if (value is short sh)
                Write(BitConverter.GetBytes(sh));

            else if (value is ushort ush)
                Write(BitConverter.GetBytes(ush));

            else if (value is int i32)
                Write(BitConverter.GetBytes(i32));

            else if (value is uint ui32)
                Write(BitConverter.GetBytes(ui32));

            else if (value is long lo)
                Write(BitConverter.GetBytes(lo));

            else if (value is ulong ulo)
                Write(BitConverter.GetBytes(ulo));

            else if (value is float fl)
                Write(BitConverter.GetBytes(fl));

            else if (value is double dou)
                Write(BitConverter.GetBytes(dou));

            else if (value is null)
                throw new ArgumentNullException(nameof(value));

            else
                throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported and does not implement {nameof(IBinarySerializable)}");
        }

        /// <summary>
        /// Writes a null-terminated string to the stream
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        public void WriteNullTerminatedString(string value, Encoding encoding = null)
        {
            // Set encoding if null
            if (encoding == null)
                encoding = Settings.StringEncoding;

            // Get the string bytes
            var bytes = encoding.GetBytes(value);

            // Write the bytes to the stream
            Write(bytes);

            // Write the null value
            Write((byte)0x00);
        }
    }
}