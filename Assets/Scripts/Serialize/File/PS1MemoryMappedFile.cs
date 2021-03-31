using BinarySerializer;

namespace R1Engine
{
    public class PS1MemoryMappedFile : MemoryMappedFile {
		public InvalidPointerMode invalidPointerMode;
		public PS1MemoryMappedFile(Context context, uint baseAddress,
			InvalidPointerMode invalidPointerMode) : base(context, baseAddress) {
			this.invalidPointerMode = invalidPointerMode;
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
			switch (invalidPointerMode) {
				case InvalidPointerMode.DevPointerXOR:
					return CheckIfDevPointer(serializedValue, anchor: anchor);
				case InvalidPointerMode.Allow:
				default:
					return true;
			}
		}

		public enum InvalidPointerMode {
			DevPointerXOR,
			Allow
		}
	}
}
