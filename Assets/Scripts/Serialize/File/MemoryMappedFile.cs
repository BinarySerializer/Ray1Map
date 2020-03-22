using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class MemoryMappedFile : BinaryFile {
		public uint length = 0;

		public MemoryMappedFile(Context context, uint baseAddress) : base(context) {
			this.baseAddress = baseAddress;
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
			if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset < baseAddress + length) {
				return new Pointer(serializedValue, this, anchor: anchor);
			}
			foreach (BinaryFile f in Context.MemoryMap.Files) {
				if (f is MemoryMappedFile) {
					Pointer p = f.GetPointer(serializedValue, anchor: anchor);
					if (p != null) return p;
				}
			}
			return null;
		}
	}
}
