using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Mode7_TileFrames : BinarySerializable
    {
        public uint TileSetFramesBlockLength { get; set; } // Set before serializing
        public bool HasPaletteIndices { get; set; } // Set before serializing

        public RGBA5551Color[] Palette { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public TileFrame[] TileFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));

            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (TileFrames == null)
            {
                var datas = new List<TileFrame>();
                var index = 0;
                long length = 4;

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
                    length += 0x200; // Palette

                do
                {
                    var frame = s.SerializeObject<TileFrame>(default, x =>
                    {
                        x.HasPaletteIndices = HasPaletteIndices;
                        x.Width = Width;
                        x.Height = Height;
                    }, name: $"{nameof(TileFrames)}[{index++}]");

                    datas.Add(frame);
                    length += frame.Size;
                } while (length < TileSetFramesBlockLength);

                TileFrames = datas.ToArray();
            }
            else
            {
                TileFrames = s.SerializeObjectArray<TileFrame>(TileFrames, TileFrames.Length, name: nameof(TileFrames));
            }
        }

        public class TileFrame : BinarySerializable
        {
            public bool HasPaletteIndices { get; set; } // Set before serializing
            public ushort Width { get; set; } // Set before serializing
            public ushort Height { get; set; } // Set before serializing

            public uint DataLength { get; set; }
            public byte[] TileSet { get; set; } // Width * Height, 4bpp
            public byte[] PaletteIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                var isCompressed = s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash2 || 
                                   s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman;

                if (isCompressed)
                    DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));

                var length = Width * Height * 0x20;
                s.DoEncodedIf(new GBAVV_Mode7_TileSetEncoder(length), isCompressed, () => TileSet = s.SerializeArray<byte>(TileSet, length, name: nameof(TileSet)));

                if (HasPaletteIndices)
                {
                    PaletteIndices = s.SerializeArray<byte>(PaletteIndices, Mathf.CeilToInt((Width * Height) / 2f), name: nameof(PaletteIndices));
                    s.Align();
                }

                if (isCompressed)
                    s.Goto(Offset + 4 + DataLength);
            }
        }
    }
}