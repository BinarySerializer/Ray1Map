using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A binary serializer used for deserializing
    /// </summary>
    public class BinaryDeserializer : BaseBinarySerializer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseStream">The base stream</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="gameSettings">The game settings</param>
        public BinaryDeserializer(Stream baseStream, string filePath, GameSettings gameSettings) : base(baseStream, filePath, gameSettings)
        { }

        /// <summary>
        /// Reads a supported value from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <returns>The value</returns>
        public T Read<T>()
        {
            // Helper method which returns an object so we can cast it
            object ReadObject()
            {
                // Get the type
                var type = typeof(T);

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        // Make sure the type implements the interface
                        if (!typeof(IBinarySerializable).IsAssignableFrom(type))
                            throw new NotSupportedException($"The specified generic type does not implement {nameof(IBinarySerializable)}");

                        // Create a new instance
                        var instance = (IBinarySerializable)Activator.CreateInstance(type);

                        // Deserialize the type
                        instance.Deserialize(this);

                        // Return the instance
                        return instance;

                    case TypeCode.Boolean:
                        var b = ReadByte();

                        if (b != 0 && b != 1)
                            Debug.LogWarning("Binary boolean was not correctly formatted");

                        return b == 1;

                    case TypeCode.SByte:
                        return (sbyte)ReadByte();

                    case TypeCode.Byte:
                        return (byte)ReadByte();

                    case TypeCode.Int16:
                        return BitConverter.ToInt16(ReadBytes(sizeof(short)), 0);

                    case TypeCode.UInt16:
                        return BitConverter.ToUInt16(ReadBytes(sizeof(ushort)), 0);

                    case TypeCode.Int32:
                        return BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);

                    case TypeCode.UInt32:
                        return BitConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);

                    case TypeCode.Int64:
                        return BitConverter.ToInt64(ReadBytes(sizeof(long)), 0);

                    case TypeCode.UInt64:
                        return BitConverter.ToUInt64(ReadBytes(sizeof(ulong)), 0);

                    case TypeCode.Single:
                        return BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);

                    case TypeCode.Double:
                        return BitConverter.ToDouble(ReadBytes(sizeof(double)), 0);

                    case TypeCode.Decimal:
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
        /// <param name="count">The amount of values to read</param>
        /// <returns>The values</returns>
        public T[] Read<T>(ulong count)
        {
            // Use byte reading method if requested
            if (typeof(T) == typeof(byte))
                return (T[])(object)ReadBytes((int)count);

            var buffer = new T[count];

            for (ulong i = 0; i < count; i++)
                // Read the value
                buffer[i] = Read<T>();

            return buffer;
        }

        /// <summary>
        /// Reads a byte from the stream
        /// </summary>
        /// <param name="xor">The xor key to use</param>
        /// <returns>The byte</returns>
        protected byte ReadByte(byte xor = 0)
        {
            // Read the byte
            var b = BaseStream.ReadByte();

            // Decrypt the byte
            if (xor != 0)
                b ^= xor;

            // Make sure it was read
            if (b == -1)
                throw new Exception("The byte could not be read");

            // Return it
            return (byte)b;
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream
        /// </summary>
        /// <param name="count">The amount of bytes to read</param>
        /// <returns>The byte</returns>
        protected byte[] ReadBytes(int count)
        {
            // Create the buffer
            var buffer = new byte[count];

            // Read into the buffer
            var readCount = BaseStream.Read(buffer, 0, count);

            // Verify the correct number of bytes were read
            if (readCount != count)
                throw new Exception("The requested number of bytes were not read");

            // Return the byte buffer
            return buffer;
        }

        /// <summary>
        /// Reads a null-terminated string from the stream
        /// </summary>
        /// <param name="encoding">The encoding to use, or null for the default one</param>
        /// <returns>The string</returns>
        public string ReadNullTerminatedString(Encoding encoding = null)
        {
            // Set encoding if null
            if (encoding == null)
                encoding = Settings.StringEncoding;

            // Use a binary reader so we can read characters
            using (var reader = new BinaryReader(BaseStream, encoding, true))
            {
                // Create the string to read to
                string str = String.Empty;

                // Current character
                char ch;

                // Read until null (0x00)
                while ((ch = reader.ReadChar()) != 0x00)
                    // Append the character
                    str += ch;

                // Return the string
                return str;
            }
        }
    }
}