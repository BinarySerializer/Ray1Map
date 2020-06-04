using System;
using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Level load command for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_LevelLoadCommand : R1Serializable
    {
        public ushort Type;

        // Arguments
        public uint UInt1;
        public uint UInt2;
        public short Short1;
        public short Short2;
        public short Short3;
        public Pointer PalettePointer;
        public Pointer ImageBufferPointer; // Compressed
        public uint ImageBufferMemoryPointer; // Uncompressed data is loaded to this location
        public uint DESDataMemoryPointer; // full DES array is copied to 0x001F9000. These pointers point to an offset in that array. Maybe it's not all DES data?
        public Pointer LevelMapBlockPointer;
        public Pointer LevelMysteriousDataPointer;
        public uint ImageBufferMemoryPointerPointer; // The address of the image buffer in memory is writtento this location. Is referenced in the DES data.


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Type = s.Serialize<ushort>(Type, name: nameof(Type));
            if (Type % 4 != 0 || Type > 0x40) {
                throw new InvalidDataException($"Level load command type {Type} at {Offset} is incorrect.");
            }
            switch (Type) {
                case 0x0: break;
                case 0x8:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    LevelMapBlockPointer = s.SerializePointer(LevelMapBlockPointer, name: nameof(LevelMapBlockPointer));
                    LevelMysteriousDataPointer = s.SerializePointer(LevelMysteriousDataPointer, name: nameof(LevelMysteriousDataPointer));
                    break;
                case 0xC:
                    // Used for vignettes/backgrounds/tiles
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    break;
                case 0x10:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
                    break;
                case 0x18:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
                    break;
                case 0x1C:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;
                case 0x20:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;
                case 0x24:
                    // Used for sprites
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
                    break;
                case 0x30:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;
                case 0x34:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;
                case 0x38:
                    PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
                    break;
                case 0x4:
                case 0x14:
                case 0x28:
                case 0x2C:
                case 0x3C:
                case 0x40:
                    throw new InvalidDataException($"Level load command type {Type} at {Offset} is not yet implemented.");
            }
        }
    }
}