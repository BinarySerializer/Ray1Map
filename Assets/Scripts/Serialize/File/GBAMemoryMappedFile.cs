namespace R1Engine.Serialize
{
    public class GBAMemoryMappedFile : MemoryMappedFile {
		public GBAMemoryMappedFile(Context context, uint baseAddress) : base(context, baseAddress) { }

        // Set to true for now since some pointers seem to be nulled out (0xFFFFFFFF)
        public override bool AllowInvalidPointer(uint serializedValue, Pointer anchor = null) => true;
    }
}
