using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class StreamFile : BinaryFile {
		public uint length = 0;
		private Stream stream;

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

		public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
			// No pointers in stream file
			/*uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset <= baseAddress + length) {
				return new Pointer(serializedValue, this, anchor: anchor);
			}*/
			// Check memory mapped files
			List<MemoryMappedFile> files = Context.MemoryMap.Files.OfType<MemoryMappedFile>().ToList<MemoryMappedFile>();
			files.Sort((a, b) => b.baseAddress.CompareTo(a.baseAddress));
			foreach (MemoryMappedFile f in files) {
				Pointer p = f.GetPointerInThisFileOnly(serializedValue, anchor: anchor);
				if (p != null) return p;
			}
			return null;
		}
	}
}
