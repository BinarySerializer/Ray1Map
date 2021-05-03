using BinarySerializer;

namespace R1Engine
{
    public class PS1MemoryMappedFile : MemoryMappedFile 
    {
        public PS1MemoryMappedFile(Context context, string filePath, uint baseAddress, InvalidPointerMode currentInvalidPointerMode, Endian endianness = Endian.Little, long fileLength = 0) : base(context, filePath, baseAddress, endianness, fileLength)
        {
            CurrentInvalidPointerMode = currentInvalidPointerMode;
        }

        public InvalidPointerMode CurrentInvalidPointerMode { get; }

		private bool CheckIfDevPointer(uint serializedValue, Pointer anchor = null) 
        {
			uint anchorOffset = anchor?.AbsoluteOffset ?? 0;
			uint offset = serializedValue + anchorOffset;
			offset ^= 0xFFFFFFFF;
			if (offset >= 0x80000000 && offset < 0x807FFFFF) {
				return true; // Probably
			}
			return false;
		}
		public override bool AllowInvalidPointer(uint serializedValue, Pointer anchor = null)
        {
            return CurrentInvalidPointerMode switch
            {
                InvalidPointerMode.DevPointerXOR => CheckIfDevPointer(serializedValue, anchor: anchor),
                InvalidPointerMode.Allow => true,
                _ => true
            };
        }

		public enum InvalidPointerMode 
        {
			DevPointerXOR,
			Allow
		}
    }
}
