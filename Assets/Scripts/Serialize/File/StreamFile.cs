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
			uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset <= baseAddress + length) {
				return new Pointer(serializedValue, this, anchor: anchor);
			}
			return null;
		}
	}
}
