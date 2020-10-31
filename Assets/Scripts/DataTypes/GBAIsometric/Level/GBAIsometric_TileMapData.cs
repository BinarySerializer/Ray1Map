using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_TileMapData : R1Serializable
    {
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer Pointer1 { get; set; } // level 0: pointer to byte array with length = lookup buffer  (0xfd5), pointer to 0x55F bytes, pointer to 0x55F ushorts
        public Pointer Pointer2 { get; set; } // ushorts
        public Pointer Pointer3 { get; set; } // ushorts. max ushort in this array = pointer2.length. not length-1, so probably either 0 or length has some special meaning

        public GBAIsometric_TileAssemble[] AssembleData { get; set; } = new GBAIsometric_TileAssemble[4];

        public Pointer PalettesPointer { get; set; }

        // Parsed

        public Pointer[] Pointer1_Pointers { get; set; }
        public ARGB1555Color[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
            Pointer3 = s.SerializePointer(Pointer3, name: nameof(Pointer3));
            for (int i = 0; i < 4; i++) {
                AssembleData[i] = s.SerializeObject<GBAIsometric_TileAssemble>(AssembleData[i], onPreSerialize: ad => ad.TileCompression = (GBAIsometric_TileAssemble.Compression)i, name: $"{nameof(AssembleData)}[{i}]");
            }
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            s.DoAt(Pointer1, () => {
                Pointer1_Pointers = s.SerializePointerArray(Pointer1_Pointers, 3, name: nameof(Pointer1_Pointers));
            });
            Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<ARGB1555Color>(Palettes, 16 * 45, name: nameof(Palettes)));

            s.DoEncoded(new RHR_SpriteEncoder(false, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));
                //Color[] cols = AnimatedPalettes.Select(c => c.GetColor());
                //Util.ByteArrayToFile(Context.BasePath + $"tiles/Full_4Bit_{Offset.StringAbsoluteOffset}.bin", fullSheet);
            });
        }
    }
}