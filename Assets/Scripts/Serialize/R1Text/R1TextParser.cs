using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Handles parsing for Mapper text files
    /// </summary>
    public class R1TextParser : IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="stream">The file stream</param>
        /// <param name="encoding">The encoding to use, or null for default</param>
        public R1TextParser(GameSettings settings, Stream stream, Encoding encoding = null)
        {
            GameSettings = settings;
            Stream = stream;
            Encoding = encoding ?? Settings.StringEncoding;
        }

        public GameSettings GameSettings { get; }
        protected Stream Stream { get; }
        protected Encoding Encoding { get; }

        protected byte CurrentXOR { get; set; }

        protected byte ReadByte()
        {
            // Read the next byte
            var bi = Stream.ReadByte();

            // Make sure it's valid
            if (bi == -1)
                throw new Exception("Mapper command byte could not be read");

            // Decrypt it
            bi ^= CurrentXOR;

            // Return as a byte
            return (byte)bi;
        }

        public string ReadValue(bool readAsString = false, Encoding encoding = null)
        {
            var buffer = new List<byte>();

            while (true)
            {
                // Read the next byte
                var b = ReadByte();

                // Stop reading if we reached the terminator ('*')
                if (b == 0x2A)
                {
                    buffer = null;
                    break;
                }

                // Skip comments ('/')
                if (b == 0x2F && !readAsString)
                {
                    do
                    {
                        b = ReadByte();
                    } while (b != 0x2F);
                }

                // Stop reading if we reached a value separator (',') or (';') for Classic
                if (GameSettings.GameModeSelection == GameModeSelection.RaymanClassicMobile)
                {
                    if (b == 0x3B)
                        break;
                }
                else
                {
                    if (b == 0x2C)
                        break;
                }

                // Ignore padding
                if (!readAsString)
                {
                    if (b <= 0x20)
                        continue;
                }
                else
                {
                    if (b < 0x20)
                        continue;
                }

                // Add to the buffer and continue reading
                buffer.Add(b);
            }

            // Return the value as a string, or null if reached the end
            return buffer != null ? (encoding ?? Encoding).GetString(buffer.ToArray()) : null;
        }
        public byte ReadByteValue() => Byte.TryParse(ReadValue(), out var b) ? b : (byte)0;
        public short ReadShortValue() => Int16.TryParse(ReadValue(), out var b) ? b : (short)0;
        public uint ReadUIntValue() => UInt32.TryParse(ReadValue(), out var b) ? b : 0;

        public void WriteString(string value)
        {
            var buffer = Encoding.GetBytes(value);

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)(buffer[i] ^ CurrentXOR);

            Stream.Write(buffer, 0, buffer.Length);
        }
        public void WriteNewLine()
        {
            Stream.WriteByte((byte)(0x0A ^ CurrentXOR));
        }

        public void BeginXOR(byte xor) => CurrentXOR = xor;
        public void EndXOR() => CurrentXOR = 0;
        public void GoTo(uint offset) => Stream.Position = offset;

        public void Dispose() => Stream?.Dispose();
    }
}