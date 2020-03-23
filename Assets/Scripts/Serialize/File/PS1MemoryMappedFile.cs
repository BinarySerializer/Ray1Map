using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Serialize {
	public class PS1MemoryMappedFile : MemoryMappedFile {
		public PS1MemoryMappedFile(Context context, uint baseAddress) : base(context, baseAddress) {
		}

		private bool CheckIfDevPointer(uint serializedValue, Pointer anchor = null) {
			uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			uint offset = serializedValue + anchorOffset;
			offset ^= 0xFFFFFFFF;
			if (offset >= 0x80000000 && offset < 0x807FFFFF) {
				return true; // Probably
			}
			return false;
		}
		public override bool AllowInvalidPointer(uint serializedValue, Pointer anchor = null) {
			return CheckIfDevPointer(serializedValue, anchor: anchor);
		}
	}
}
