using BinarySerializer;

namespace R1Engine
{
    public class GBAMemoryMappedFile : MemoryMappedFile 
    {
        public GBAMemoryMappedFile(Context context, string filePath, uint baseAddress, Endian endianness = Endian.Little, long fileLength = 0) : base(context, filePath, baseAddress, endianness, fileLength)
        { }

        // Set to true for now since some pointers seem to be nulled out (0xFFFFFFFF)
        public override bool AllowInvalidPointer(long serializedValue, Pointer anchor = null) => true;
    }
}
