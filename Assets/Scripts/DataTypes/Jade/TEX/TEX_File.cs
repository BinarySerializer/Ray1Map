using BinarySerializer;
using System;
using UnityEngine;

namespace R1Engine.Jade
{
    // See: GDI_l_AttachWorld & TEX_l_File_Read
    // TEX_l_File_Read reads the texture header, after that GDI_l_AttachWorld resolves some references (for animated textures) and adds palette references
    public class TEX_File : Jade_File {
        public bool IsContent { get; set; }
        public bool HasContent
            => FileFormat != TexFileFormat.RawPal
            && FileFormat != TexFileFormat.Procedural
            && FileFormat != TexFileFormat.SpriteGen;

        public bool CanHaveFontDesc
            => FileFormat == TexFileFormat.Tga
            || FileFormat == TexFileFormat.Bmp
            || FileFormat == TexFileFormat.Jpeg
            || FileFormat == TexFileFormat.Raw
            || FileFormat == TexFileFormat.Xenon;

        public int Int_00 { get; set; } // Always 0xFFFFFFFF in files
        public ushort Flags { get; set; }
        public TexFileFormat FileFormat { get; set; }
        public TexColorFormat ColorFormat { get; set; } // Determines bits per pixel
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint Uint_0C { get; set; }
        public Jade_Reference<STR_FontDescriptor> FontDesc { get; set; }
        public Jade_Code Code_14 { get; set; } // Usually CAD01234
        public Jade_Code Code_18 { get; set; } // Checked for 0xFF00FF
        public Jade_Code Code_1C { get; set; } // Checked for 0xC0DEC0DE

        public TEX_Content_RawPal Content_RawPal { get; set; }
        public TGA Content_TGA { get; set; }
        public TEX_Content_Procedural Content_Procedural { get; set; }
        public MAT_SpriteGen Content_SpriteGen { get; set; }
        public byte[] Content { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			if (!Loader.IsBinaryData)
				s.Goto(s.CurrentPointer + FileSize - 32);

			Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            if (Int_00 == -1) {
                Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
                FileFormat = s.Serialize<TexFileFormat>(FileFormat, name: nameof(FileFormat));
                ColorFormat = s.Serialize<TexColorFormat>(ColorFormat, name: nameof(ColorFormat));
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
                Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));
                FontDesc = s.SerializeObject<Jade_Reference<STR_FontDescriptor>>(FontDesc, name: nameof(FontDesc));
                Code_14 = s.Serialize<Jade_Code>(Code_14, name: nameof(Code_14));
                Code_18 = s.Serialize<Jade_Code>(Code_18, name: nameof(Code_18));
                Code_1C = s.Serialize<Jade_Code>(Code_1C, name: nameof(Code_1C));

                if (!Loader.IsBinaryData)
                    s.Goto(Offset);

                bool hasReadContent = false;
                uint contentSize = FileSize - (uint)(s.CurrentPointer - Offset);
                switch (FileFormat) 
                {
                    case TexFileFormat.RawPal:
                        if (FileSize > 0x50 || (FileSize & 0x3) != 0) {
                            throw new NotImplementedException($"TEX_File: Load header for type {FileFormat}");
                        }
                        Content_RawPal = s.SerializeObject<TEX_Content_RawPal>(Content_RawPal, c => c.Texture = this, name: nameof(Content_RawPal));
                        break;

                    case TexFileFormat.Procedural:
                        if (contentSize > 0) {
                            Content_Procedural = s.SerializeObject<TEX_Content_Procedural>(Content_Procedural, onPreSerialize: c => c.FileSize = contentSize, name: nameof(Content_Procedural));
                            hasReadContent = true;
                        }
                        break;

                    case TexFileFormat.SpriteGen:
                        if (contentSize > 0) {
                            Content_SpriteGen = s.SerializeObject<MAT_SpriteGen>(Content_SpriteGen, name: nameof(Content_SpriteGen));
                        }
                        hasReadContent = true;
                        break;

                    case TexFileFormat.Animated:
                        throw new NotImplementedException($"TEX_File: Load header for type {FileFormat}");

                    case TexFileFormat.Tga:
                        if (IsContent)
                        {
                            TGA.RGBColorFormat colorFormat;

                            switch (s.GetR1Settings().EngineVersion)
                            {
                                case EngineVersion.Jade_RRR_PC:
                                case EngineVersion.Jade_BGE_PC:
                                    colorFormat = TGA.RGBColorFormat.BGR;
                                    break;

                                case EngineVersion.Jade_RRR_Xbox360:
                                case EngineVersion.Jade_RRR_PS2:
                                case EngineVersion.Jade_BGE_PS2:
                                default:
                                    colorFormat = TGA.RGBColorFormat.RGB;
                                    break;
                            }

                            Content_TGA = s.SerializeObject<TGA>(Content_TGA, x => x.ColorFormat = colorFormat, name: nameof(Content_TGA));
                            hasReadContent = true;
                        }
                        break;

                    case TexFileFormat.Raw:
                    case TexFileFormat.Jpeg:
                    case TexFileFormat.Bmp:
                    case TexFileFormat.Xenon:
                    default:
                        if (IsContent) {
                            Content = s.SerializeArray<byte>(Content, FileSize - (s.CurrentPointer - Offset), name: nameof(Content));
                            if (Content.Length > 0) 
                                hasReadContent = true;
                        }
                        break;
                }
                if (hasReadContent && (Flags & 0x40) != 0 && CanHaveFontDesc) {
                    FontDesc?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
                }
            }
        }

        public Texture2D ToTexture2D()
        {
            return FileFormat switch
            {
                TexFileFormat.Raw => null, // Gets parsed from RawPal
                TexFileFormat.SpriteGen => null, // Points to a RawPal
                TexFileFormat.RawPal => Content_RawPal.References.FindItem(t => t.HasTexture)?.ToTexture2D(),
                TexFileFormat.Tga => Content_TGA.ToTexture2D(),
                TexFileFormat.Jpeg => ToTexture2DFromJpeg(),
                _ => throw new NotImplementedException($"TODO: Implement texture type {FileFormat}")
            };
        }

        public Texture2D ToTexture2DFromJpeg()
        {
            var tex = TextureHelpers.CreateTexture2D(Width, Height);

            tex.LoadImage(Content);

            return tex;
        }

        public enum TexFileFormat : byte
        {
            Tga = 1,
            Bmp = 2,
            Jpeg = 3,
            SpriteGen = 4,
            Procedural = 5,
            Raw = 6,
            RawPal = 7,
            Animated = 9,
            Xenon = 11,
        }

        public enum TexColorFormat : byte
        {
            BPP_4 = 0x50,
            BPP_8 = 0x40,

            BPP_16 = 0x31,
            BPP_24 = 0x20,
            BPP_32 = 0x10,
        }
	}
}