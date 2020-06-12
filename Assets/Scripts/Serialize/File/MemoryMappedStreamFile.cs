using System.IO;

namespace R1Engine.Serialize
{
    public class MemoryMappedStreamFile : MemoryMappedFile
    {
        public MemoryMappedStreamFile(string name, Stream stream, Context context, uint baseAddress) : base(context, baseAddress)
        {
            filePath = name;
            this.Stream = stream;
        }

        public override uint Length
        {
            get => (uint)Stream.Length;
            set => Stream.SetLength(value);
        }

        private Stream Stream { get; }

        public override Reader CreateReader()
        {
            Reader reader = new Reader(Stream, isLittleEndian: Endianness == Endian.Little);
            return reader;
        }

        public override Writer CreateWriter()
        {
            Writer writer = new Writer(Stream, isLittleEndian: Endianness == Endian.Little);
            Stream.Position = 0;
            return writer;
        }
    }
}