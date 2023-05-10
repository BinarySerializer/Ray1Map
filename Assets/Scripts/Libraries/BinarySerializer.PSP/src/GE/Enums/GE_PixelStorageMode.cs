namespace BinarySerializer.PSP
{
    public enum GE_PixelStorageMode : uint {
		RGBA5650 = 0x00,
		RGBA5551 = 0x01,
		RGBA4444 = 0x02,
		RGBA8888 = 0x03,
		Index4 = 0x04,
		Index8 = 0x05,
		Index16 = 0x06,
		Index32 = 0x07,
		DXT1 = 0x08,
		DXT3 = 0x09,
		DXT5 = 0x0A,
	}
}