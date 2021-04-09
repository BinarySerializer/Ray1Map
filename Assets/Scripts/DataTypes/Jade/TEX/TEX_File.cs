using BinarySerializer;
using System;

namespace R1Engine.Jade
{
    // See: GDI_l_AttachWorld & TEX_l_File_Read
    // TEX_l_File_Read reads the texture header, after that GDI_l_AttachWorld resolves some references (for animated textures) and adds palette references
    public class TEX_File : Jade_File {
        public bool IsContent { get; set; }
        public bool HasContent => FileFormat != TexFileFormat.RawPal;
        public bool CanHaveFontDesc => FileFormat == TexFileFormat.Tga
            || FileFormat == TexFileFormat.Bmp
            || FileFormat == TexFileFormat.Jpeg
            || FileFormat == TexFileFormat.Raw
            || FileFormat == TexFileFormat.Xenon;

        public int Int_00 { get; set; } // Always 0xFFFFFFFF in files
        public ushort Flags { get; set; }
        public TexFileFormat FileFormat { get; set; }
        public byte ColorFormat { get; set; } // Determines bits per pixel
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint Uint_0C { get; set; }
        public Jade_Reference<STR_FontDescriptor> FontDesc { get; set; }
        public Jade_Code Code_14 { get; set; } // Usually CAD01234
        public Jade_Code Code_18 { get; set; } // Checked for 0xFF00FF
        public Jade_Code Code_1C { get; set; } // Checked for 0xC0DEC0DE

        public byte[] Content_Raw { get; set; }
        public TEX_Content_RawPal Content_RawPal { get; set; }
        public TGA Content_TGA { get; set; }

        public byte[] Content { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
			if (!Loader.IsBinaryData)
				s.Goto(s.CurrentPointer + FileSize - 32);

			Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            if (Int_00 == -1) {
                Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
                FileFormat = s.Serialize<TexFileFormat>(FileFormat, name: nameof(FileFormat));
                ColorFormat = s.Serialize<byte>(ColorFormat, name: nameof(ColorFormat));
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
                switch (FileFormat) 
                {
                    case TexFileFormat.Raw:
                        Content_Raw = s.SerializeArray<byte>(Content_Raw, FileSize - (s.CurrentPointer - Offset), name: nameof(Content_Raw));
                        if (Content_Raw.Length > 0) 
                            hasReadContent = true;
                        break;

                    case TexFileFormat.RawPal:
                        if (FileSize > 0x50 || (FileSize & 0x3) != 0) {
                            throw new NotImplementedException($"TEX_File: Load header for type {FileFormat}");
                        }
                        Content_RawPal = s.SerializeObject<TEX_Content_RawPal>(Content_RawPal, c => c.Texture = this, name: nameof(Content_RawPal));
                        break;
                    // Types 4, 5, 9 and 7 are loaded regardless of IsContent
                    case TexFileFormat.SpriteGen:
                    case TexFileFormat.Procedural:
                    case TexFileFormat.Animated:
                        throw new NotImplementedException($"TEX_File: Load header for type {FileFormat}");

                    case TexFileFormat.Tga:
                        if (IsContent)
                        {
                            Content_TGA = s.SerializeObject<TGA>(Content_TGA, name: nameof(Content_TGA));
                            hasReadContent = true;
                        }

                        break;

                    case TexFileFormat.Bmp:
                    case TexFileFormat.Jpeg:
                    case TexFileFormat.Xenon:
                    default:
                        if (IsContent) {
                            Content = s.SerializeArray<byte>(Content, FileSize - (s.CurrentPointer - Offset), name: nameof(Content));
                            if (Content.Length > 0) hasReadContent = true;
                            //throw new NotImplementedException($"TODO: Implement texture type {FileFormat}");
                        }
                        break;
                    //default:
                    //    break;//throw new NotImplementedException($"TODO: Implement texture type {FileFormat}");
                }
                if (hasReadContent && (Flags & 0x40) != 0 && CanHaveFontDesc) {
                    FontDesc?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
                }

            }
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
	}
}