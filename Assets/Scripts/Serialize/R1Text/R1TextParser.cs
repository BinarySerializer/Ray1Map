using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        protected byte? BufferByte { get; set; }

        public bool SupportsComments { get; set; } = true;
        public bool SeparateAtPadding { get; set; } = false;

        protected byte? ReadByte()
        {
            // Read the next byte
            var bi = BufferByte ?? Stream.ReadByte();

            BufferByte = null;

            // Make sure it's valid
            if (bi == -1)
                return null;

            // Decrypt it
            bi ^= CurrentXOR;

            // Return as a byte
            return (byte)bi;
        }

        // This is a combination of the different file parsing functions in RayKit
        public string ReadValue(bool readAsString = false, Encoding encoding = null)
        {
            // Make sure we're not at the end of the file
            if (Stream.Position >= Stream.Length)
                return null;

            var buffer = new List<byte>();

            while (true)
            {
                // Read the next byte
                var b = ReadByte();

                // Make sure we're not at the end of the file
                if (b == null)
                    break;

                // Stop reading if we reached the terminator ('*')
                if (b == 0x2A)
                {
                    buffer = null;
                    break;
                }

                // Skip comments ('/')
                if (b == 0x2F && !readAsString && SupportsComments)
                {
                    do
                    {
                        b = ReadByte();
                    } while (b != 0x2F && b != null);

                    b = ReadByte();

                    // Make sure we're not at the end of the file
                    if (b == null)
                        break;
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
                    {
                        // Read remaining padding
                        while (b <= 0x20)
                            b = ReadByte();

                        // Make sure we're not at the end of the file
                        if (b == null)
                            break;

                        // Save first valid byte for next read
                        BufferByte = b;

                        // Only break if we have read something and are separate at padding
                        if (SeparateAtPadding && buffer.Any())
                            break;
                        else
                            continue;
                    }
                }
                else
                {
                    if (b < 0x20)
                        continue;
                }

                // Add to the buffer and continue reading
                buffer.Add(b.Value);
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