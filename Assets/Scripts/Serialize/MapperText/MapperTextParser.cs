using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Handles parsing for Mapper text files
    /// </summary>
    public class MapperTextParser : IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="encoding">The encoding to use, or null for default</param>
        public MapperTextParser(Stream stream, Encoding encoding = null)
        {
            Stream = stream;
            Encoding = encoding ?? Settings.StringEncoding;
        }

        protected Stream Stream { get; }
        protected Encoding Encoding { get; }

        public string ReadValue()
        {
            var buffer = new List<byte>();

            while (true)
            {
                // Read the next byte
                var bi = Stream.ReadByte();

                // Make sure it's valid
                if (bi == -1)
                    throw new Exception("Mapper command byte could not be read");

                // Cast to a byte
                var b = (byte)bi;

                // Stop reading if we reached the terminator ('*')
                if (b == 0x2A)
                {
                    buffer = null;
                    break;
                }

                // Skip comments ('/')
                if (b == 0x2F)
                {
                    do
                    {
                        b = (byte)Stream.ReadByte();
                    } while (b != 0x2F);
                }

                // Stop reading if we reached a value separator (',')
                if (b == 0x2C)
                    break;

                // Ignore padding
                if (b <= 0x20)
                    continue;

                // Add to the buffer and continue reading
                buffer.Add(b);
            }

            // Return the value as a string, or null if reached the end
            return buffer != null ? Encoding.GetString(buffer.ToArray()) : null;
        }
        public byte ReadByteValue() => Byte.TryParse(ReadValue(), out var b) ? b : (byte)0;
        public short ReadShortValue() => Int16.TryParse(ReadValue(), out var b) ? b : (short)0;
        public void WriteString(string value)
        {
            var buffer = Encoding.GetBytes(value);
            Stream.Write(buffer, 0, buffer.Length);
        }
        public void WriteNewLine()
        {
            Stream.WriteByte(0x0A);
        }

        public void Dispose() => Stream?.Dispose();
    }
}