using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class LinearSerializedFile : BinaryFile {
		public uint length = 0;

		public LinearSerializedFile(Context context) : base(context) {
		}

		public override Pointer StartPointer => new Pointer((uint)baseAddress, this);

		public override Reader CreateReader() {
			Stream s = FileSystem.GetFileReadStream(AbsolutePath);
			length = (uint)s.Length;
			Reader reader = new Reader(s, isLittleEndian: true);
			return reader;
		}

		public override Writer CreateWriter() {
			CreateBackupFile();
			Stream s = FileSystem.GetFileWriteStream(AbsolutePath);
			length = (uint)s.Length;
			Writer writer = new Writer(s, isLittleEndian: true);
			return writer;
		}

		public override Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
			if (length == 0) {
				Stream s = FileSystem.GetFileReadStream(AbsolutePath);
				length = (uint)s.Length;
				s.Close();
			}
			uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset <= baseAddress + length) {
				return new Pointer(serializedValue, this, anchor: anchor);
			}
			return null;
		}
	}
}
