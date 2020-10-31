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

        public Pointer Pointer4 { get; set; } // 35320 bytes?
        public Pointer Pointer4_DataOffsetsPointer { get; set; } // Compressed

        public Pointer Pointer6 { get; set; }
        public Pointer Pointer6_DataOffsetsPointer { get; set; } // Compressed

        public Pointer Pointer8 { get; set; }
        public Pointer Pointer8_DataOffsetsPointer { get; set; } // Compressed

        public Pointer Pointer10 { get; set; }
        public Pointer Pointer10_DataOffsetsPointer { get; set; } // Compressed

        public Pointer PalettesPointer { get; set; }

        // Parsed

        public Pointer[] Pointer1_Pointers { get; set; }
        public ushort[] Pointer4_DataOffsets { get; set; } // All offsets should be multiplied by 4 to get the byte offset
        public ushort[] Pointer6_DataOffsets { get; set; }
        public ushort[] Pointer8_DataOffsets { get; set; }
        public ushort[] Pointer10_DataOffsets { get; set; }
        public ARGB1555Color[] Palettes { get; set; }


        public byte[][] Pointer4_Data { get; set; }
        public byte[][] Pointer6_Data { get; set; }
        public GBAIsometric_TileMapData_TileData8[] Pointer8_Data { get; set; }
        public GBAIsometric_TileMapData_TileData10[] Pointer10_Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
            Pointer3 = s.SerializePointer(Pointer3, name: nameof(Pointer3));
            Pointer4 = s.SerializePointer(Pointer4, name: nameof(Pointer4));
            Pointer4_DataOffsetsPointer = s.SerializePointer(Pointer4_DataOffsetsPointer, name: nameof(Pointer4_DataOffsetsPointer));
            Pointer6 = s.SerializePointer(Pointer6, name: nameof(Pointer6));
            Pointer6_DataOffsetsPointer = s.SerializePointer(Pointer6_DataOffsetsPointer, name: nameof(Pointer6_DataOffsetsPointer));
            Pointer8 = s.SerializePointer(Pointer8, name: nameof(Pointer8));
            Pointer8_DataOffsetsPointer = s.SerializePointer(Pointer8_DataOffsetsPointer, name: nameof(Pointer8_DataOffsetsPointer));
            Pointer10 = s.SerializePointer(Pointer10, name: nameof(Pointer10));
            Pointer10_DataOffsetsPointer = s.SerializePointer(Pointer10_DataOffsetsPointer, name: nameof(Pointer10_DataOffsetsPointer));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));

            s.DoAt(Pointer1, () => {
                Pointer1_Pointers = s.SerializePointerArray(Pointer1_Pointers, 3, name: nameof(Pointer1_Pointers));
            });
            s.DoAt(Pointer4_DataOffsetsPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => Pointer4_DataOffsets = s.SerializeArray<ushort>(Pointer4_DataOffsets, s.CurrentLength / 2, name: nameof(Pointer4_DataOffsets)));
            });
            s.DoAt(Pointer6_DataOffsetsPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => Pointer6_DataOffsets = s.SerializeArray<ushort>(Pointer6_DataOffsets, s.CurrentLength / 2, name: nameof(Pointer6_DataOffsets)));
            });
            s.DoAt(Pointer8_DataOffsetsPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => Pointer8_DataOffsets = s.SerializeArray<ushort>(Pointer8_DataOffsets, s.CurrentLength / 2, name: nameof(Pointer8_DataOffsets)));
            });
            s.DoAt(Pointer10_DataOffsetsPointer, () =>
            {
                s.DoEncoded(new RHREncoder(), () => Pointer10_DataOffsets = s.SerializeArray<ushort>(Pointer10_DataOffsets, s.CurrentLength / 2, name: nameof(Pointer10_DataOffsets)));
            });
            Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<ARGB1555Color>(Palettes, 16 * 45, name: nameof(Palettes)));

            s.DoEncoded(new RHR_SpriteEncoder(false, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
                byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));
                //Color[] cols = AnimatedPalettes.Select(c => c.GetColor());
                //Util.ByteArrayToFile(Context.BasePath + $"tiles/Full_4Bit_{Offset.StringAbsoluteOffset}.bin", fullSheet);
            });


            if (Pointer4_Data == null) {
                Pointer4_Data = new byte[Pointer4_DataOffsets.Length][];
                for (int i = 0; i < Pointer4_DataOffsets.Length; i++) {
                    Pointer nextOff = i < Pointer4_DataOffsets.Length - 1 ? (Pointer4 + Pointer4_DataOffsets[i + 1] * 4) : Pointer4_DataOffsetsPointer;
                    s.DoAt(Pointer4 + Pointer4_DataOffsets[i] * 4, () => {
                        Pointer4_Data[i] = s.SerializeArray<byte>(Pointer4_Data[i], nextOff - s.CurrentPointer, name: $"{nameof(Pointer4_Data)}[{i}]");
                    });
                }
            }
            if (Pointer6_Data == null) {
                Pointer6_Data = new byte[Pointer6_DataOffsets.Length][];
                for (int i = 0; i < Pointer6_DataOffsets.Length; i++) {
                    Pointer nextOff = i < Pointer6_DataOffsets.Length - 1 ? (Pointer6 + Pointer6_DataOffsets[i + 1] * 4) : Pointer6_DataOffsetsPointer;
                    s.DoAt(Pointer6 + Pointer6_DataOffsets[i] * 4, () => {
                        Pointer6_Data[i] = s.SerializeArray<byte>(Pointer6_Data[i], nextOff - s.CurrentPointer, name: $"{nameof(Pointer6_Data)}[{i}]");
                    });
                }
            }
            if (Pointer8_Data == null) {
                Pointer8_Data = new GBAIsometric_TileMapData_TileData8[Pointer8_DataOffsets.Length];
                for (int i = 0; i < Pointer8_DataOffsets.Length; i++) {
                    //Pointer nextOff = i < Pointer8_DataOffsets.Length - 1 ? (Pointer8 + Pointer8_DataOffsets[i + 1] * 4) : Pointer8_DataOffsetsPointer;
                    s.DoAt(Pointer8 + Pointer8_DataOffsets[i] * 4, () => {
                        Pointer8_Data[i] = s.SerializeObject<GBAIsometric_TileMapData_TileData8>(Pointer8_Data[i], name: $"{nameof(Pointer8_Data)}[{i}]");
                    });
                }
            }
            if (Pointer10_Data == null) {
                Pointer10_Data = new GBAIsometric_TileMapData_TileData10[Pointer10_DataOffsets.Length];
                for (int i = 0; i < Pointer10_DataOffsets.Length; i++) {
                    //Pointer nextOff = i < Pointer10_DataOffsets.Length - 1 ? (Pointer10 + Pointer10_DataOffsets[i + 1] * 4) : Pointer10_DataOffsetsPointer;
                    s.DoAt(Pointer10 + Pointer10_DataOffsets[i] * 4, () => {
                        Pointer10_Data[i] = s.SerializeObject<GBAIsometric_TileMapData_TileData10>(Pointer10_Data[i], name: $"{nameof(Pointer10_Data)}[{i}]");
                    });
                }
            }
        }
    }
}