using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class GBACrash_Mode7_TileFrames : R1Serializable
    {
        public uint TileSetFramesBlockLength { get; set; } // Set before serializing
        public bool HasPaletteIndices { get; set; } // Set before serializing

        public RGBA5551Color[] Palette { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public TileFrame[] TileFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1)
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));

            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (TileFrames == null)
            {
                var datas = new List<TileFrame>();
                var index = 0;
                long length = 4;

                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash1)
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

        public class TileFrame : R1Serializable
        {
            public bool HasPaletteIndices { get; set; } // Set before serializing
            public ushort Width { get; set; } // Set before serializing
            public ushort Height { get; set; } // Set before serializing

            public uint DataLength { get; set; }
            public byte[] Crash2_Header { get; set; }
            public byte[] TileSet { get; set; } // Width * Height, 4bpp
            public byte[] PaletteIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                if (s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2)
                {
                    DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
                    Crash2_Header = s.SerializeArray<byte>(Crash2_Header, 4, name: nameof(Crash2_Header));
                }

                TileSet = s.SerializeArray<byte>(TileSet, s.GameSettings.EngineVersion == EngineVersion.GBACrash_Crash2 ? (long)(DataLength - 4) : Width * Height * 0x20, name: nameof(TileSet));

                if (HasPaletteIndices)
                {
                    PaletteIndices = s.SerializeArray<byte>(PaletteIndices, Mathf.CeilToInt((Width * Height) / 2f), name: nameof(PaletteIndices));
                    s.Align();
                }
            }
        }
    }
}