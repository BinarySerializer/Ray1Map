using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A binary data serializer
    /// </summary>
    public class BinarySerializer
    {
        #region Constructors

        static BinarySerializer()
        {
            ChecksumCalculators = new Dictionary<string, IChecksumCalculator>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mode">The serializer mode</param>
        /// <param name="baseStream">The base stream</param>
        /// <param name="instance">The object instance</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="gameSettings">The game settings</param>
        public BinarySerializer(SerializerMode mode, Stream baseStream, object instance, string filePath, GameSettings gameSettings)
        {
            Mode = mode;
            BaseStream = baseStream;
            Instance = instance;
            InstanceType = Instance.GetType();
            FilePath = filePath;
            GameSettings = gameSettings;
        }

        #endregion

        #region Serializer Methods

        /// <summary>
        /// Serializes the specified property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="xorKey">The xor key to use</param>
        public void Serialize(string propertyName, byte xorKey = 0)
        {
            // Get the property
            var prop = InstanceType.GetProperty(propertyName);

            if (prop == null)
                throw new Exception($"No property found with the name {propertyName}");

            if (Mode == SerializerMode.Write)
            {
                // Get the value
                var value = prop.GetValue(Instance);

                // Write the value to the stream
                Write(value, xorKey);
            }
            else
            {
                // Read the value
                var value = Read(prop.PropertyType, xorKey);

                // Set the value
                prop.SetValue(Instance, value);
            }
        }

        /// <summary>
        /// Serializes the specified property
        /// </summary>
        /// <typeparam name="T">The type of items in the array</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="readCount">The amount of items to read</param>
        /// <param name="xorKey">The xor key to use</param>
        public void SerializeArray<T>(string propertyName, long readCount, byte xorKey = 0)
        {
            // Get the property
            var prop = InstanceType.GetProperty(propertyName);

            if (prop == null)
                throw new Exception($"No property found with the name {propertyName}");

            if (Mode == SerializerMode.Write)
            {
                // Get the value
                var value = prop.GetValue(Instance);

                // Write the value to the stream
                Write(value);
            }
            else
            {
                // Read the value
                var value = ReadArray<T>(readCount, xorKey);

                // Set the value
                prop.SetValue(Instance, value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The base stream
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// The object instance
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// The instance type
        /// </summary>
        public Type InstanceType { get; }

        /// <summary>
        /// The serializer mode
        /// </summary>
        public SerializerMode Mode { get; }

        /// <summary>
        /// The path of the file being serialized
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The file name of the file being serialized, in lower-case
        /// </summary>
        public string FileName => Path.GetFileName(FilePath)?.ToLower();

        /// <summary>
        /// The file extensions of the file being serialized, in lower-case
        /// </summary>
        public string FileExtension => Path.GetExtension(FilePath)?.ToLower();

        /// <summary>
        /// The game settings
        /// </summary>
        public GameSettings GameSettings { get; }

        #endregion

        #region Read Methods

        /// <summary>
        /// Reads a supported value from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <param name="xorKey">The xor key to use</param>
        /// <returns>The value</returns>
        public T Read<T>(byte xorKey = 0) => (T)Read(typeof(T), xorKey);

        /// <summary>
        /// Reads a supported value from the stream
        /// </summary>
        /// <param name="valueType">The type of value to read</param>
        /// <param name="xorKey">The xor key to use</param>
        /// <returns>The value</returns>
        public object Read(Type valueType, byte xorKey = 0)
        {
            // Get the type code
            var typeCode = Type.GetTypeCode(valueType);

            // Treat enums as bytes
            if (valueType.IsEnum)
                typeCode = TypeCode.Byte;

            switch (typeCode)
            {
                case TypeCode.Object:
                    // Make sure the type implements the interface
                    if (!typeof(IBinarySerializable).IsAssignableFrom(valueType))
                        throw new NotSupportedException($"The specified generic type does not implement {nameof(IBinarySerializable)}");

                    // Create a new instance
                    var instance = (IBinarySerializable)Activator.CreateInstance(valueType);

                    // Deserialize the type
                    instance.Serialize(new BinarySerializer(Mode, BaseStream, instance, FilePath, GameSettings));

                    // Return the instance
                    return instance;

                case TypeCode.Boolean:
                    var b = ReadByte(xorKey);

                    if (b != 0 && b != 1)
                        Debug.LogWarning("Binary boolean was not correctly formatted");

                    return b == 1;

                case TypeCode.SByte:
                    return (sbyte)ReadByte(xorKey);

                case TypeCode.Byte:
                    return ReadByte(xorKey);

                case TypeCode.Int16:
                    return BitConverter.ToInt16(ReadBytes(sizeof(short), xorKey), 0);

                case TypeCode.UInt16:
                    return BitConverter.ToUInt16(ReadBytes(sizeof(ushort), xorKey), 0);

                case TypeCode.Int32:
                    return BitConverter.ToInt32(ReadBytes(sizeof(int), xorKey), 0);

                case TypeCode.UInt32:
                    return BitConverter.ToUInt32(ReadBytes(sizeof(uint), xorKey), 0);

                case TypeCode.Int64:
                    return BitConverter.ToInt64(ReadBytes(sizeof(long), xorKey), 0);

                case TypeCode.UInt64:
                    return BitConverter.ToUInt64(ReadBytes(sizeof(ulong), xorKey), 0);

                case TypeCode.Single:
                    return BitConverter.ToSingle(ReadBytes(sizeof(float), xorKey), 0);

                case TypeCode.Double:
                    return BitConverter.ToDouble(ReadBytes(sizeof(double), xorKey), 0);

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

        /// <summary>
        /// Reads the remaining bytes from the stream
        /// </summary>
        /// <returns>The remaining bytes</returns>
        public byte[] ReadRemainingBytes()
        {
            return ReadArray<byte>(BaseStream.Length - BaseStream.Position);
        }

        /// <summary>
        /// Reads an array of supported values from the stream
        /// </summary>
        /// <typeparam name="T">The type of value to read</typeparam>
        /// <param name="count">The amount of values to read</param>
        /// <param name="xorKey">The xor key to use</param>
        /// <returns>The values</returns>
        public T[] ReadArray<T>(long count, byte xorKey = 0)
        {
            // Use byte reading method if requested
            if (typeof(T) == typeof(byte))
                return (T[])(object)ReadBytes((int)count, xorKey);

            var buffer = new T[count];

            for (long i = 0; i < count; i++)
                // Read the value
                buffer[i] = xorKey == 0 ? Read<T>() : Read<T>(xorKey);

            return buffer;
        }

        /// <summary>
        /// Reads a byte from the stream
        /// </summary>
        /// <param name="xorKey">The xor key to use</param>
        /// <returns>The byte</returns>
        protected byte ReadByte(byte xorKey = 0)
        {
            // Read the byte
            var b = BaseStream.ReadByte();

            // Make sure it was read
            if (b == -1)
                throw new Exception("The byte could not be read");

            // Decrypt the byte
            if (xorKey != 0)
                b ^= xorKey;

            // Cast to a byte
            var bb = (byte)b;

            // Add to the checksum
            if (ChecksumCalculators.ContainsKey(FilePath))
                ChecksumCalculators[FilePath].AddByte(bb);

            // Return it
            return bb;
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream
        /// </summary>
        /// <param name="count">The amount of bytes to read</param>
        /// <param name="xorKey">The xor key to use</param>
        /// <returns>The byte</returns>
        protected byte[] ReadBytes(int count, byte xorKey = 0)
        {
            // Create the buffer
            var buffer = new byte[count];

            // Read into the buffer
            var readCount = BaseStream.Read(buffer, 0, count);

            // Verify the correct number of bytes were read
            if (readCount != count)
                throw new Exception("The requested number of bytes were not read");

            // Decrypt the bytes
            if (xorKey != 0)
            {
                for (int i = 0; i < count; i++)
                    buffer[i] ^= xorKey;
            }

            // Add to the checksum
            if (ChecksumCalculators.ContainsKey(FilePath))
                ChecksumCalculators[FilePath].AddBytes(buffer);

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

                // TODO: Add to checksum

                // Read until null (0x00)
                while ((ch = reader.ReadChar()) != 0x00)
                    // Append the character
                    str += ch;

                // Return the string
                return str;
            }
        }

        #endregion

        #region Checksum

        /// <summary>
        /// The current checksum calculators
        /// </summary>
        protected static Dictionary<string, IChecksumCalculator> ChecksumCalculators { get; set; }

        /// <summary>
        /// Begins calculating byte checksum for all decrypted bytes read from the stream
        /// </summary>
        /// <param name="checksumCalculator">The checksum calculator to use</param>
        public void BeginCalculateChecksum(IChecksumCalculator checksumCalculator)
        {
            ChecksumCalculators.Add(FilePath, checksumCalculator);
        }

        /// <summary>
        /// Ends calculating the checksum and return the value
        /// </summary>
        /// <typeparam name="T">The type of checksum value</typeparam>
        /// <returns>The checksum value</returns>
        public T EndCalculateChecksum<T>()
        {
            // Get the calculator
            var cc = (IChecksumCalculator<T>)ChecksumCalculators[FilePath];

            // Remove the entry
            ChecksumCalculators.Remove(FilePath);

            // Return the value
            return cc.ChecksumValue;
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes and encrypts a supported value to the stream
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="xorKey">The xor key to use</param>
        public void Write(object value, byte xorKey = 0)
        {
            if (value is IBinarySerializable serializable)
                serializable.Serialize(new BinarySerializer(Mode, BaseStream, serializable, FilePath, GameSettings));

            else if(value is byte[] ba)
                WriteBytes(ba, xorKey);

            else if (value is Array a)
                foreach (var item in a)
                    Write(item, xorKey);

            else if (value is bool bo)
                BaseStream.WriteByte((byte)((bo ? 1 : 0) ^ xorKey));

            else if (value is sbyte sb)
                BaseStream.WriteByte((byte)(sb ^ xorKey));

            else if (value.GetType().IsEnum)
                BaseStream.WriteByte((byte)((int)value ^ xorKey));

            else if (value is byte by)
                BaseStream.WriteByte((byte)(by ^ xorKey));

            else if (value is short sh)
                Write(BitConverter.GetBytes(sh), xorKey);

            else if (value is ushort ush)
                Write(BitConverter.GetBytes(ush), xorKey);

            else if (value is int i32)
                Write(BitConverter.GetBytes(i32), xorKey);

            else if (value is uint ui32)
                Write(BitConverter.GetBytes(ui32), xorKey);

            else if (value is long lo)
                Write(BitConverter.GetBytes(lo), xorKey);

            else if (value is ulong ulo)
                Write(BitConverter.GetBytes(ulo), xorKey);

            else if (value is float fl)
                Write(BitConverter.GetBytes(fl), xorKey);

            else if (value is double dou)
                Write(BitConverter.GetBytes(dou), xorKey);

            else if (value is null)
                throw new ArgumentNullException(nameof(value));

            else
                throw new NotSupportedException($"The specified type {value.GetType().Name} is not supported and does not implement {nameof(IBinarySerializable)}");
        }

        /// <summary>
        /// Writes the specified bytes to the stream
        /// </summary>
        /// <param name="bytes">The bytes to write</param>
        /// <param name="xorKey">The xor key to use</param>
        public void WriteBytes(byte[] bytes, byte xorKey)
        {
            // Write the bytes normally if not encrypted
            if (xorKey == 0)
            {
                BaseStream.Write(bytes, 0, bytes.Length);
            }
            // Otherwise encrypt to new array
            else
            {
                // Encrypt to a new array so the original doesn't get modified
                var encrypted = new byte[bytes.Length];

                // Encrypt every byte
                for (int i = 0; i < bytes.Length; i++)
                    encrypted[i] = (byte)(bytes[i] ^ xorKey);

                // Write the encrypted bytes
                BaseStream.Write(bytes, 0, bytes.Length);
            }
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

        #endregion
    }
}