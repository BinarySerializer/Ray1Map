﻿using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_JTX : BinarySerializable {
        public TEX_File Texture { get; set; }

        public uint Version { get; set; }
        public JTX_Format Format { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public int HasMipmaps { get; set; }
        public float MipmapBias { get; set; }

        public Jade_PaletteReference Palette { get; set; }

        public int BPP { get; set; }
        public uint MipmapSize { get; set; }

        public uint PoPSoT_UInt0 { get; set; }
        public uint PoPSoT_UInt1 { get; set; }
        public uint PoPSoT_UInt2 { get; set; }
        public uint PoPSoT_UInt3 { get; set; }
        public uint PoPSoT_Size { get; set; }
        public byte[] PoPSoT_Content { get; set; }

        public byte[] Content { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = (uint)(Texture.FileSize - (s.CurrentPointer - Texture.Offset));
            if (FileSize == 0) return;
            
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (Version == 0) return;
			Format = s.Serialize<JTX_Format>(Format, name: nameof(Format));
			Width = s.Serialize<uint>(Width, name: nameof(Width));
			Height = s.Serialize<uint>(Height, name: nameof(Height));
			HasMipmaps = s.Serialize<int>(HasMipmaps, name: nameof(HasMipmaps));
			if (Version >= 3) MipmapBias = s.Serialize<float>(MipmapBias, name: nameof(MipmapBias));

            uint paletteKey = Format switch {
                JTX_Format.Alpha_8 => 0x24001bfe,
                JTX_Format.Intensity_8 => 0x24001bff,
                JTX_Format.Alpha_4 => 0x24002029,
                JTX_Format.Intensity_4 => 0x2400202b,
                JTX_Format.AlphaIntensity_8 => 0x24002028,
                JTX_Format.AlphaIntensity_4 => 0x2400202a,
                _ => 0
            };
            if (paletteKey != 0) Palette = new Jade_PaletteReference(s.Context, new Jade_Key(s.Context, paletteKey))?.Resolve();

            switch (Format) {
                case JTX_Format.Raw32:
                    BPP = 32;
                    break;
                case JTX_Format.Palette_4:
                case JTX_Format.Palette_8:
                    BPP = Format == JTX_Format.Palette_4 ? 4 : 8;
					Palette = s.SerializeObject<Jade_PaletteReference>(Palette, name: nameof(Palette))?.Resolve();
					break;
                case JTX_Format.Intensity_8:
                    BPP = 8;
                    break;
                case JTX_Format.Intensity_4:
                    BPP = 4;
                    break;
                default:
                    throw new NotImplementedException($"Unimplemented TEX_Content_JTX Format: {Format}");
            }

            if (Texture.IsContent) {
                if (s.GetR1Settings().EngineVersion != EngineVersion.Jade_PoP_SoT || !Loader.IsBinaryData) {
                    Content = s.SerializeArray<byte>(Content, BPP * Height * Width / 8 + MipmapSize, name: nameof(Content));
                }

                if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_PoP_SoT && Version >= 2) {
                    PoPSoT_UInt0 = s.Serialize<uint>(PoPSoT_UInt0, name: nameof(PoPSoT_UInt0));
                    if (PoPSoT_UInt0 == 0) return;
                    PoPSoT_UInt1 = s.Serialize<uint>(PoPSoT_UInt1, name: nameof(PoPSoT_UInt1));
                    PoPSoT_UInt2 = s.Serialize<uint>(PoPSoT_UInt2, name: nameof(PoPSoT_UInt2));
                    PoPSoT_UInt3 = s.Serialize<uint>(PoPSoT_UInt3, name: nameof(PoPSoT_UInt3));
                    PoPSoT_Size = s.Serialize<uint>(PoPSoT_Size, name: nameof(PoPSoT_Size));
                    PoPSoT_Content = s.SerializeArray<byte>(PoPSoT_Content, PoPSoT_Size, name: nameof(PoPSoT_Content));
                }
            }
		}

        public enum JTX_Format : uint {
            Raw32 = 0,
            Palette_8 = 1,
            Palette_4 = 2,
            Alpha_8 = 3,
            Intensity_8 = 4,
            S3TC = 5,
            DXT3 = 6,
            DXT5 = 7,
            Alpha_4 = 8,
            Intensity_4 = 9,
            AlphaIntensity_8 = 10,
            AlphaIntensity_4 = 11,
            S3TC_A = 12,

            // Added for NCIS
            DXN = 13,
            JTX_Format_DXN_DXT5A = 14
        }
	}
}