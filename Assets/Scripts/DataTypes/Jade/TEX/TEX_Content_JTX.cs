using System;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_JTX : BinarySerializable {
        public TEX_File Texture { get; set; }

        public uint Version { get; set; }
        public JTX_Format Format { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public int MipmapCount { get; set; }
        public float MipmapBias { get; set; }

        public Jade_PaletteReference Palette { get; set; }

        public int BPP { get; set; }
        public uint MipmapSize { get; set; }
        public uint ContentSize { get; set; }
        public uint Content2Size { get; set; }

        public uint PS2_UInt0 { get; set; }
        public uint PS2_UInt1 { get; set; }
        public uint PS2_UInt2 { get; set; }
        public uint PS2_IsSwizzled { get; set; }
        public uint PS2_Size { get; set; }
        public TEX_Content_JTX_PS2 PS2_Content { get; set; }

        public byte[] Content { get; set; }
        public byte[] Content2 { get; set; }
        public int HasContentOverride { get; set; } // Boolean
        public uint ContentOverrideSize { get; set; }
        public byte[] ContentOverride { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = (uint)(Texture.FileSize - (s.CurrentPointer - Texture.Offset));
            if (FileSize == 0) return;
            
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (Version == 0) return;
			Format = s.Serialize<JTX_Format>(Format, name: nameof(Format));
			Width = s.Serialize<uint>(Width, name: nameof(Width));
			Height = s.Serialize<uint>(Height, name: nameof(Height));
			MipmapCount = s.Serialize<int>(MipmapCount, name: nameof(MipmapCount));
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
                case JTX_Format.AlphaIntensity_8:
                    BPP = 8;
                    break;
                case JTX_Format.Intensity_4:
                case JTX_Format.AlphaIntensity_4:
                    BPP = 4;
                    break;
                case JTX_Format.S3TC:
                case JTX_Format.S3TC_A:
                    BPP = 4;
                    break;
                case JTX_Format.DXT3:
                case JTX_Format.DXT5:
                    BPP = 8;
                    break;
                default:
                    throw new NotImplementedException($"Unimplemented TEX_Content_JTX Format: {Format}");
            }
            CalculateContentSize();

            if (Texture.IsContent) {
                if (s.GetR1Settings().Platform != Platform.PS2 || !Loader.IsBinaryData) {
                    Content = s.SerializeArray<byte>(Content, ContentSize, name: nameof(Content));
					if (Content2Size > 0) Content2 = s.SerializeArray<byte>(Content2, Content2Size, name: nameof(Content2));
				}

                if (s.GetR1Settings().Platform == Platform.PS2 && Version >= 2) {
                    PS2_UInt0 = s.Serialize<uint>(PS2_UInt0, name: nameof(PS2_UInt0));
                    if (PS2_UInt0 == 0) return;
                    PS2_UInt1 = s.Serialize<uint>(PS2_UInt1, name: nameof(PS2_UInt1));
                    PS2_UInt2 = s.Serialize<uint>(PS2_UInt2, name: nameof(PS2_UInt2));
                    PS2_IsSwizzled = s.Serialize<uint>(PS2_IsSwizzled, name: nameof(PS2_IsSwizzled));
                    PS2_Size = s.Serialize<uint>(PS2_Size, name: nameof(PS2_Size));
					PS2_Content = s.SerializeObject<TEX_Content_JTX_PS2>(PS2_Content, onPreSerialize: cont => cont.JTX = this, name: nameof(PS2_Content));
                }

                if (Version >= 2 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_SoT)) {
					HasContentOverride = s.Serialize<int>(HasContentOverride, name: nameof(HasContentOverride));
                    if (HasContentOverride == 1 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs)) {
						ContentOverrideSize = s.Serialize<uint>(ContentOverrideSize, name: nameof(ContentOverrideSize));
						ContentOverride = s.SerializeArray<byte>(ContentOverride, ContentOverrideSize, name: nameof(ContentOverride));
					}
				}
            }
		}

        public void CalculateContentSize() {
            // Calculate mipmap size
            uint bpp = (uint)BPP;
            MipmapSize = 0;
            uint cur_w = Width;
            uint cur_h = Height;
            for (int i = 0; i < MipmapCount; i++) {
                if (cur_w > 8) cur_w >>= 1;
                if (cur_h > 8) cur_h >>= 1;
                MipmapSize += (cur_w / 8) * cur_h * bpp;
            }

            switch (Format) {
                case JTX_Format.DXT3:
                case JTX_Format.DXT5:
                    ContentSize = (uint)((Height >> 2) * (Width >> 2) * 16 + MipmapSize);
                    break;
                case JTX_Format.S3TC:
                case JTX_Format.S3TC_A:
                    ContentSize = (uint)((Height >> 2) * (Width >> 2) * 8 + MipmapSize);
                    if(Format == JTX_Format.S3TC_A) Content2Size = ContentSize;
                    break;
                default:
                    ContentSize = (uint)(BPP * Height * Width / 8 + MipmapSize);
                    break;
            }
        }

        public enum JTX_Format : uint {
            Raw32 = 0,
            Palette_8 = 1,
            Palette_4 = 2,
            Alpha_8 = 3,
            Intensity_8 = 4,
            S3TC = 5, // DXT1?
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

        public Texture2D ToTexture2D() {
            Texture2D tex = null;
            DDS dds = null;
            byte[] content = Content;
            if (PS2_Content != null) {
                if (PS2_IsSwizzled != 0) {
                    content = new byte[PS2_Content.Content.Length];
                    Array.Copy(PS2_Content.Content, content, content.Length);
                    if (BPP == 4) {
                        ezSwizzle.writeTexPSMCT32(0, (int)Width / 128, 0, 0, (int)PS2_Content.Width, (int)PS2_Content.Height, content);
                        Array.Resize<byte>(ref content, (int)(Height * Width));
                        ezSwizzle.readTexPSMT4_mod(0, (int)Width / 64, 0, 0, (int)Width, (int)Height, ref content);
                    } else {
                        ezSwizzle.writeTexPSMCT32(0, (int)Width / 128, 0, 0, (int)PS2_Content.Width, (int)PS2_Content.Height, content);
                        //texData = new byte[h.height * h.width];
                        ezSwizzle.readTexPSMT8(0, (int)Width / 64, 0, 0, (int)Width, (int)Height, ref content);
                    }
                } else {
                    content = PS2_Content.Content;
                }
            }
            Color[] palette = Palette?.Value != null ? Palette.Value.Colors.Select(c => c.GetColor()).ToArray() : null;
            switch (Format) {
                case JTX_Format.Palette_8:
                case JTX_Format.Alpha_8:
                case JTX_Format.AlphaIntensity_8:
                case JTX_Format.Intensity_8:
                    tex = TextureHelpers.CreateTexture2D((int)Width, (int)Height);
                    tex.FillRegion(content, 0, palette, Util.TileEncoding.Linear_8bpp, 0, 0, (int)Width, (int)Height);
                    break;
                case JTX_Format.Palette_4:
                case JTX_Format.Alpha_4:
                case JTX_Format.AlphaIntensity_4:
                case JTX_Format.Intensity_4:
                    tex = TextureHelpers.CreateTexture2D((int)Width, (int)Height);
                    if (PS2_IsSwizzled != 0) {
                        tex.FillRegion(content, 0, palette, Util.TileEncoding.Linear_8bpp, 0, 0, (int)Width, (int)Height);
                    } else {
                        if (Context.GetR1Settings().Platform != Platform.PS2) {
                            tex.FillRegion(content, 0, palette, Util.TileEncoding.Linear_4bpp_ReverseOrder, 0, 0, (int)Width, (int)Height);
                        } else {
                            tex.FillRegion(content, 0, palette, Util.TileEncoding.Linear_4bpp, 0, 0, (int)Width, (int)Height);
                        }
                    }
                    break;
                case JTX_Format.Raw32:
                    tex = TextureHelpers.CreateTexture2D((int)Width, (int)Height);
                    var tileEncoding = Util.TileEncoding.Linear_32bpp_RGBA;
                    if(Context.GetR1Settings().Platform != Platform.PS2) tileEncoding = Util.TileEncoding.Linear_32bpp_BGRA;
                    tex.FillRegion(content, 0, palette, tileEncoding, 0, 0, (int)Width, (int)Height);
                    break;
                case JTX_Format.S3TC:
                    dds = DDS.FromRawData(content, DDS_Parser.PixelFormat.DXT1, Width, Height);
                    tex = dds.PrimaryTexture?.ToTexture2D();
                    break;
                case JTX_Format.S3TC_A:
                    dds = DDS.FromRawData(Content, DDS_Parser.PixelFormat.DXT1, Width, Height);
                    tex = dds.PrimaryTexture?.ToTexture2D();
                    var dds_a = DDS.FromRawData(Content2, DDS_Parser.PixelFormat.DXT1, Width, Height);
                    var tex_a = dds_a.PrimaryTexture?.ToTexture2D();
                    if (tex_a != null) {
                        var pixels_base = tex.GetPixels();
                        var pixels_a = tex_a.GetPixels();
                        for (int i = 0; i < pixels_base.Length; i++) {
                            var p = pixels_base[i];
                            var p_a = pixels_a[i];
                            pixels_base[i] = new Color(p.r, p.g, p.b, p_a.r);
                        }
                        tex.SetPixels(pixels_base);
                        tex.Apply();
                    }
                    break;
                case JTX_Format.DXT3:
                    dds = DDS.FromRawData(content, DDS_Parser.PixelFormat.DXT3, Width, Height);
                    tex = dds.PrimaryTexture?.ToTexture2D();
                    break;
                case JTX_Format.DXT5:
                    dds = DDS.FromRawData(content, DDS_Parser.PixelFormat.DXT5, Width, Height);
                    tex = dds.PrimaryTexture?.ToTexture2D();
                    break;
                default:
                    throw new NotImplementedException($"TODO: Implement JTX type {Format}");
            }
            return tex;
        }
	}
}