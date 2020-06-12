using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Level load command for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_LevelLoadCommand : R1Serializable
    {
        public LevelLoadCommandType Type { get; set; }

        // Arguments
        public uint UInt1 { get; set; }
        public uint UInt2 { get; set; }
        public short Short1 { get; set; }
        public short Short2 { get; set; }
        public short Short3 { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer ImageBufferPointer { get; set; } // Compressed
        public uint ImageBufferMemoryPointer { get; set; } // Uncompressed data is loaded to this location
        public uint DESDataMemoryPointer { get; set; } // full DES array is copied to 0x001F9000. These pointers point to an offset in that array. Maybe it's not all DES data?
        public Pointer LevelMapBlockPointer { get; set; }
        public Pointer LevelEventBlockPointer { get; set; }
        public uint ImageBufferMemoryPointerPointer { get; set; } // The address of the image buffer in memory is writtento this location. Is referenced in the DES data.
        public uint TargetImageBufferMemoryPointer { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) 
        {
            // Serialize the type
            Type = s.Serialize<LevelLoadCommandType>(Type, name: nameof(Type));

            // Make sure the type is valid
            if ((ushort)Type % 4 != 0 || (ushort)Type > 0x40)
                throw new InvalidDataException($"Level load command type {Type} at {Offset} is incorrect.");
            
            // Parse the data based on the type
            switch (Type) 
            {
                case LevelLoadCommandType.End:
                    break;
                
                case LevelLoadCommandType.LevelMap:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    LevelMapBlockPointer = s.SerializePointer(LevelMapBlockPointer, name: nameof(LevelMapBlockPointer));
                    LevelEventBlockPointer = s.SerializePointer(LevelEventBlockPointer, name: nameof(LevelEventBlockPointer));
                    break;

                case LevelLoadCommandType.Graphics:
                    // Used for vignettes/backgrounds/tiles
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    break;

                case LevelLoadCommandType.Unk1:
                    UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
                    UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
                    break;

                case LevelLoadCommandType.MoveGraphics:
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    TargetImageBufferMemoryPointer = s.Serialize<uint>(TargetImageBufferMemoryPointer, name: nameof(TargetImageBufferMemoryPointer));
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;

                case LevelLoadCommandType.Unk3:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    break;

                case LevelLoadCommandType.UnkDES1:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;

                case LevelLoadCommandType.Sprites:
                    // Used for sprites
                    ImageBufferPointer = s.SerializePointer(ImageBufferPointer, name: nameof(ImageBufferPointer));
                    ImageBufferMemoryPointer = s.Serialize<uint>(ImageBufferMemoryPointer, name: nameof(ImageBufferMemoryPointer));
                    ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
                    break;

                case LevelLoadCommandType.UnkDES2:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    Short3 = s.Serialize<short>(Short3, name: nameof(Short3));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;

                case LevelLoadCommandType.UnkDES3:
                    Short1 = s.Serialize<short>(Short1, name: nameof(Short1));
                    Short2 = s.Serialize<short>(Short2, name: nameof(Short2));
                    DESDataMemoryPointer = s.Serialize<uint>(DESDataMemoryPointer, name: nameof(DESDataMemoryPointer));
                    break;

                case LevelLoadCommandType.Palette:
                    PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
                    break;

                case LevelLoadCommandType.NotImplemented1:
                case LevelLoadCommandType.NotImplemented2:
                case LevelLoadCommandType.NotImplemented3:
                case LevelLoadCommandType.NotImplemented4:
                case LevelLoadCommandType.NotImplemented5:
                case LevelLoadCommandType.NotImplemented6:
                    throw new InvalidDataException($"Level load command type {Type} at {Offset} is not yet implemented.");
            }
        }

        public enum LevelLoadCommandType : ushort
        {
            End = 0x00,
            NotImplemented1 = 0x04,
            LevelMap = 0x08,
            Graphics = 0x0C,
            Unk1 = 0x10,
            NotImplemented2 = 0x14,
            MoveGraphics = 0x18,
            Unk3 = 0x1C,
            UnkDES1 = 0x20,
            Sprites = 0x24,
            NotImplemented3 = 0x28,
            NotImplemented4 = 0x2C,
            UnkDES2 = 0x30,
            UnkDES3 = 0x34,
            Palette = 0x38,
            NotImplemented5 = 0x3C,
            NotImplemented6 = 0x40,

        }
    }
}