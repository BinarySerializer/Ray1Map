using System;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Extension methods for a <see cref="Stream"/>
    /// </summary>
    public static class StreamExtensions
    {
        #region Common

        /// <summary>
        /// Reads a supported value from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <param name="stream">The stream to read from</param>
        /// <returns>The value</returns>
        public static T Read<T>(this Stream stream)
        {
            // Helper method which returns an object so we can cast it
            object ReadObject()
            {
                // Get the type
                var type = typeof(T);

                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        // Make sure the type implements the interface
                        if (!typeof(ISerializableFile).IsAssignableFrom(type))
                            throw new NotSupportedException($"The specified generic type does not implement {nameof(ISerializableFile)}");

                        // Create a new instance
                        var instance = (ISerializableFile)Activator.CreateInstance(type);

                        // Deserialize the type
                        instance.Deserialize(stream);

                        // Return the instance
                        return instance;

                    case TypeCode.Boolean:
                        return stream.ReadByte() == 1;

                    case TypeCode.SByte:
                        throw new NotImplementedException();

                    case TypeCode.Byte:
                        return (byte)stream.ReadByte();

                    case TypeCode.Int16:
                        return BitConverter.ToInt16(stream.ReadBytes(sizeof(short)), 0);

                    case TypeCode.UInt16:
                        return BitConverter.ToUInt16(stream.ReadBytes(sizeof(ushort)), 0);

                    case TypeCode.Int32:
                        return BitConverter.ToInt32(stream.ReadBytes(sizeof(int)), 0);

                    case TypeCode.UInt32:
                        return BitConverter.ToUInt32(stream.ReadBytes(sizeof(uint)), 0);

                    case TypeCode.Int64:
                        return BitConverter.ToInt64(stream.ReadBytes(sizeof(long)), 0);

                    case TypeCode.UInt64:
                        return BitConverter.ToUInt64(stream.ReadBytes(sizeof(ulong)), 0);

                    case TypeCode.Single:
                        return BitConverter.ToSingle(stream.ReadBytes(sizeof(float)), 0);

                    case TypeCode.Double:
                        return BitConverter.ToDouble(stream.ReadBytes(sizeof(double)), 0);

                    case TypeCode.Decimal:
                        throw new NotImplementedException();

                    case TypeCode.String:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    default:
                        throw new NotSupportedException("The specified generic type can not be read from the reader");
                }
            }

            // Return the object cast to the generic type
            return (T)ReadObject();
        }

        /// <summary>
        /// Reads an array of supported values from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <param name="stream">The stream to read from</param>
        /// <param name="count">The amount of values to read</param>
        /// <returns>The values</returns>
        public static T[] Read<T>(this Stream stream, ulong count)
        {
            var buffer = new T[count];

            for (ulong i = 0; i < count; i++)
                // Read the value
                buffer[i] = stream.Read<T>();

            return buffer;
        }

        /// <summary>
        /// Writes a supported value to the stream
        /// </summary>
        /// <typeparam name="T">The type of value to write</typeparam>
        /// <param name="stream">The stream to read from</param>
        /// <param name="value">The value</param>
        public static void Write<T>(this Stream stream, T value)
        {
            // TODO: Check if value is an array, if so enumerate it

            if (value is ISerializableFile serializable)
                serializable.Serialize(stream);

            else if (value is bool bo)
                stream.WriteByte((byte)(bo ? 1 : 0));

            else if (value is sbyte sb)
                throw new NotImplementedException();

            else if (value is byte[] ba)
                stream.Write(ba, 0, ba.Length);

            else if (value is byte by)
                stream.WriteByte(by);

            else if (value is short sh)
                stream.Write(BitConverter.GetBytes(sh));

            else if (value is ushort ush)
                stream.Write(BitConverter.GetBytes(ush));

            else if (value is int i32)
                stream.Write(BitConverter.GetBytes(i32));

            else if (value is uint ui32)
                stream.Write(BitConverter.GetBytes(ui32));

            else if (value is long lo)
                stream.Write(BitConverter.GetBytes(lo));

            else if (value is ulong ulo)
                stream.Write(BitConverter.GetBytes(ulo));

            else if (value is float fl)
                stream.Write(BitConverter.GetBytes(fl));

            else if (value is double dou)
                stream.Write(BitConverter.GetBytes(dou));

            else if (value == null)
                throw new ArgumentNullException(nameof(value));

            else
                throw new NotSupportedException($"The specified generic type {typeof(T).Name} is not supported and does not implement {nameof(ISerializableFile)}");
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="count">The amount of bytes to read</param>
        /// <returns>The byte</returns>
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var buffer = new byte[count];

            stream.Read(buffer, 0, count);

            return buffer;
        }

        #endregion

        #region Strings

        /// <summary>
        /// Reads a null-terminated string from the stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        /// <returns>The string</returns>
        public static string ReadNullTerminatedString(this Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Settings.StringEncoding;

            using (var reader = new BinaryReader(stream, encoding, true))
            {
                string str = String.Empty;

                char ch;

                while ((ch = reader.ReadChar()) != 0)
                    str += ch;

                return str;
            }
        }

        /// <summary>
        /// Writes a null-terminated string to the stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="value">The value to write</param>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        public static void WriteNullTerminatedString(this Stream stream, string value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Settings.StringEncoding;

            // Get the string bytes
            var bytes = encoding.GetBytes(value);

            // Write the bytes to the stream
            stream.Write(bytes);

            // Write the null value
            stream.Write((byte)0x00);
        }

        #endregion
    }
}