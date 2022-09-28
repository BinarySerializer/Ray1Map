using System;
using BinarySerializer.Nintendo.GBA;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class TileKitFile : OnyxFile
    {
        public ushort TileCount_4bpp { get; set; }
        public ushort TileCount_8bpp { get; set; }
        public bool IsCompressed { get; set; } // Unused
        public byte? Byte_05 { get; set; } // Leftover animated tile kits index value from GBA format
        public byte Byte_06 { get; set; } // Unused
        public byte Byte_07 { get; set; } // Unused

        public byte PalettesCount { get; set; }
        public FileReference<PaletteFile>[] PaletteReferences { get; set; }

        public uint CompressedDataLength { get; set; }
        public byte[] TileSet_4bpp { get; set; }
        public byte[] TileSet_8bpp { get; set; }

        public override void SerializeFile(SerializerObject s)
        {
            TileCount_4bpp = s.Serialize<ushort>(TileCount_4bpp, name: nameof(TileCount_4bpp));
            TileCount_8bpp = s.Serialize<ushort>(TileCount_8bpp, name: nameof(TileCount_8bpp));
            IsCompressed = s.Serialize<bool>(IsCompressed, name: nameof(IsCompressed));
            Byte_05 = s.SerializeNullable<byte>(Byte_05, name: nameof(Byte_05));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            
            PalettesCount = s.Serialize<byte>(PalettesCount, name: nameof(PalettesCount));
            PaletteReferences = s.SerializeObjectArray<FileReference<PaletteFile>>(PaletteReferences, PalettesCount, name: nameof(PaletteReferences));

            if (Byte_05 != null)
            {
                return;
                throw new NotImplementedException("Not implemented animated tile kits");
            }

            CompressedDataLength = s.Serialize<uint>(CompressedDataLength, name: nameof(CompressedDataLength));

            // The data is always compressed no matter the bool value (at least for RRR2)
            s.DoEncoded(new LZSS_16_Encoder(), () =>
            {
                TileSet_4bpp = s.SerializeArray<byte>(TileSet_4bpp, TileCount_4bpp * 0x20, name: nameof(TileSet_4bpp));
                TileSet_8bpp = s.SerializeArray<byte>(TileSet_8bpp, TileCount_8bpp * 0x40, name: nameof(TileSet_8bpp));
            });
        }

        public override void ResolveDependencies(SerializerObject s)
        {
            PaletteReferences?.Resolve(s);
        }
    }
}