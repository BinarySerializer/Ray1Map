using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine.Serialize
{
    public class StreamFile : BinaryFile {
		public uint length = 0;
		private Stream stream;
		public bool AllowLocalPointers { get; set; }

		public StreamFile(string name, Stream stream, Context context) : base(context) {
			filePath = name;
			this.stream = stream;
			length = (uint)stream.Length;
		}

		public override Pointer StartPointer => new Pointer((uint)baseAddress, this);

		public override Reader CreateReader() {
			Reader reader = new Reader(stream, isLittleEndian: Endianness == Endian.Little);
			return reader;
		}

		public override Writer CreateWriter() {
			Writer writer = new Writer(stream, isLittleEndian: Endianness == Endian.Little);
			stream.Position = 0;
			return writer;
		}

		public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) 
        {
			// If we allow local pointers we assume the pointer leads to the stream file
			if (AllowLocalPointers)
            {
                uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
				if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset <= baseAddress + length)
					return new Pointer(serializedValue, this, anchor: anchor);
				else
					return null;
            }
			else
            {
                // Get every memory mapped file
                List<MemoryMappedFile> files = Context.MemoryMap.Files.OfType<MemoryMappedFile>().ToList();

				files.Sort((a, b) => b.baseAddress.CompareTo(a.baseAddress));
                return files.Select(f => f.GetPointerInThisFileOnly(serializedValue, anchor: anchor)).FirstOrDefault(p => p != null);
            }
		}
	}
}
