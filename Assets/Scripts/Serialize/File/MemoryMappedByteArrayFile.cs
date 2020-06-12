using System;
using System.IO;

namespace R1Engine.Serialize
{
    public class MemoryMappedByteArrayFile : MemoryMappedFile
    {
        public MemoryMappedByteArrayFile(string name, uint length, Context context, uint baseAddress) : base(context, baseAddress)
        {
            filePath = name;
            Bytes = new byte[length];
        }

        public override uint Length
        {
            get => (uint)Bytes.Length;
        }

        public byte[] Bytes { get; }

        public override Reader CreateReader()
        {
            Reader reader = new Reader(new MemoryStream(Bytes), isLittleEndian: Endianness == Endian.Little);
            return reader;
        }

        public override Writer CreateWriter()
        {
            Writer writer = new Writer(new MemoryStream(Bytes), isLittleEndian: Endianness == Endian.Little);
            return writer;
        }

        public override void EndRead(Stream readStream) {
            base.EndRead(readStream);
            readStream.Close();
        }

        public void WriteBytes(uint position, byte[] source) {
            Array.Copy(source, 0, Bytes, position, Math.Min(source.Length, Bytes.Length-position));
        }
    }
}