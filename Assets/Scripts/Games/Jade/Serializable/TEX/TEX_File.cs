using BinarySerializer;
using System;
using BinarySerializer.Image;
using System.Linq;
using UnityEngine;

namespace Ray1Map.Jade
{
    // See: GDI_l_AttachWorld & TEX_l_File_Read
    // TEX_l_File_Read reads the texture header, after that GDI_l_AttachWorld resolves some references (for animated textures) and adds palette references
    public class TEX_File : Jade_File {
		public override string Export_Extension {
            get {
                switch (Type) {
                    case TexFileType.RawPal: return "tex";
                    case TexFileType.Raw: return "raw";
                    case TexFileType.Tga: return "tga";
                    case TexFileType.Bmp: return "bmp";
                    case TexFileType.JTX: return "jtx";
                    case TexFileType.DDS: return "dds";
                    default: return null;
                }
            }
        }
		public bool IsContent { get; set; }
        public bool HasContent
            => (Type != TexFileType.RawPal || (IsRawPalUnsupported(Context) && ContentKey != null)) // On Xbox 360 it resolves the raw texture
            && Type != TexFileType.Procedural
            && Type != TexFileType.SpriteGen
            && Type != TexFileType.Animated
            && (Type != TexFileType.Raw || !IsRawPalUnsupported(Context));

        public Jade_Key ContentKey {
            get {
                if (IsRawPalUnsupported(Context) && Type == TexFileType.RawPal) {
                    return Content_RawPal?.PreferredSlot.TextureRef.Key;
                } else {
                    return Key;
                }
            }
        }
        public static bool IsRawPalUnsupported(Context c) =>
            (c.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) ||
            (c.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE && c.GetR1Settings().Platform == Platform.PC) ||
            (c.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD);
        public TEX_File Info { get; set; } // Set in onPreSerialize

        public bool CanHaveFontDesc
            => Type == TexFileType.Tga
            || Type == TexFileType.Bmp
            || Type == TexFileType.Jpeg
            || Type == TexFileType.Raw
            || Type == TexFileType.DDS
            || Type == TexFileType.JTX;

        public Jade_Key Montreal_Key { get; set; }
        public int Mark { get; set; } // Always 0xFFFFFFFF in files
        public ushort Flags { get; set; }
        public TexFileType Type { get; set; }
        public TexColorFormat Format { get; set; } // Determines bits per pixel
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public Jade_Color Color { get; set; }
        public Jade_Reference<STR_FontDescriptor> FontDesc { get; set; }
        public Jade_Code Code_14 { get; set; } = Jade_Code.CAD01234; // Usually CAD01234
        public Jade_Code Code_18 { get; set; } = Jade_Code.FF00FF; // Checked for 0xFF00FF
        public Jade_Code Code_1C { get; set; } = Jade_Code.CodeCode; // Checked for 0xC0DEC0DE

        public uint ContentSize { get; set; }
        public TEX_Content_RawPal Content_RawPal { get; set; }
        public TGA_Header Content_TGA_Header { get; set; }
        public TGA Content_TGA { get; set; }
        public TEX_Content_Procedural Content_Procedural { get; set; }
        public TEX_Content_Animated Content_Animated { get; set; }
        public MAT_SpriteGen Content_SpriteGen { get; set; }
        public TEX_Content_Xenon Content_Xenon { get; set; }
        public TEX_Content_JTX Content_JTX { get; set; }
        public DDS_Header Content_DDS_Header { get; set; }
        public DDS Content_DDS { get; set; }
        public byte[] Content { get; set; }

        protected override void SerializeFile(SerializerObject s) 
        {
			if (!Loader.IsBinaryData)
				s.Goto(s.CurrentPointer + FileSize - 32);

            if(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Loader.IsBinaryData)
				Montreal_Key = s.SerializeObject<Jade_Key>(Montreal_Key, name: nameof(Montreal_Key));
			Mark = s.Serialize<int>(Mark, name: nameof(Mark));
            if (Mark == -1) {
                Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
                Type = s.Serialize<TexFileType>(Type, name: nameof(Type));
                Format = s.Serialize<TexColorFormat>(Format, name: nameof(Format));
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
                Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
                FontDesc = s.SerializeObject<Jade_Reference<STR_FontDescriptor>>(FontDesc, name: nameof(FontDesc));
                Code_14 = s.Serialize<Jade_Code>(Code_14, name: nameof(Code_14));
                Code_18 = s.Serialize<Jade_Code>(Code_18, name: nameof(Code_18));
                Code_1C = s.Serialize<Jade_Code>(Code_1C, name: nameof(Code_1C));

                if (!Loader.IsBinaryData)
                    s.Goto(Offset);

                bool hasReadContent = false;
                ContentSize = Loader.IsBinaryData ? (FileSize - (uint)(s.CurrentPointer - Offset)) : (FileSize - 32);
                switch (Type) 
                {
                    case TexFileType.RawPal:
                        Content_RawPal = s.SerializeObject<TEX_Content_RawPal>(Content_RawPal, c => c.Texture = this, name: nameof(Content_RawPal));
                        break;

                    case TexFileType.Procedural:
                        if (ContentSize > 0) {
                            Content_Procedural = s.SerializeObject<TEX_Content_Procedural>(Content_Procedural, onPreSerialize: c => c.FileSize = ContentSize, name: nameof(Content_Procedural));
                            hasReadContent = true;
                        }
                        break;

                    case TexFileType.SpriteGen:
                        if (ContentSize > 0) {
                            Content_SpriteGen = s.SerializeObject<MAT_SpriteGen>(Content_SpriteGen, name: nameof(Content_SpriteGen));
                        }
                        hasReadContent = true;
                        break;

                    case TexFileType.Animated:
                        Content_Animated = s.SerializeObject<TEX_Content_Animated>(Content_Animated, c => c.Texture = this, name: nameof(Content_Animated));
                        break;

                    case TexFileType.JTX:
                        if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                            if (ContentSize > 0) {
                                Content_JTX = s.SerializeObject<TEX_Content_JTX>(Content_JTX, c => c.Texture = this, name: nameof(Content_JTX));
                            }
                            if (IsContent || !Loader.IsBinaryData) hasReadContent = true;
                        } else {
                            if (IsContent || !Loader.IsBinaryData) {
                                Content = s.SerializeArray<byte>(Content, ContentSize, name: nameof(Content));
                                if (Content.Length > 0)
                                    hasReadContent = true;
                            }
                        }
                        break;

                    case TexFileType.Tga:
                        if (IsContent || !Loader.IsBinaryData)
                        {
                            TGA.RGBColorOrder colorOrder = TGA.RGBColorOrder.BGR;
                            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
                                colorOrder = TGA.RGBColorOrder.RGB;
                                if (s.GetR1Settings().Platform == Platform.PC && s.GetR1Settings().EngineVersion != EngineVersion.Jade_KingKong) {
                                    colorOrder = TGA.RGBColorOrder.BGR;
                                }
                                else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Phoenix)) colorOrder = TGA.RGBColorOrder.BGR;
                                else if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE_HD) colorOrder = TGA.RGBColorOrder.BGR;
                                switch (s.GetR1Settings().GameModeSelection) {
                                    case GameModeSelection.RaymanRavingRabbidsWiiJP:
                                    case GameModeSelection.KingKongXbox360_20050926:
                                        colorOrder = TGA.RGBColorOrder.BGR;
                                        break;
                                }
                            }


                            // Serialize the header first, then set a custom one for the TGA struct based on the Jade properties instead
                            Content_TGA_Header = s.SerializeObject<TGA_Header>(Content_TGA_Header, x => x.Pre_ForceNoColorMap = true, name: nameof(Content_TGA_Header));
                            
                            if (Content_TGA != null
                                && Content_TGA.Pre_ColorOrder != colorOrder
                                && Content_TGA.RGBImageData != null) {
                                // Serializing with different color order. Convert colors
                                switch (Content_TGA.Header.BitsPerPixel) {
                                    case 24:
                                        if (colorOrder == TGA.RGBColorOrder.BGR) {
                                            Content_TGA.RGBImageData = (BaseColor[])Content_TGA.RGBImageData.Select(c => new BGR888Color(c.Red, c.Green, c.Blue)).ToArray();
                                        } else {
                                            Content_TGA.RGBImageData = (BaseColor[])Content_TGA.RGBImageData.Select(c => new RGB888Color(c.Red, c.Green, c.Blue)).ToArray();
                                        }
                                        break;
                                    case 32:
                                        if (colorOrder == TGA.RGBColorOrder.BGR) {
                                            Content_TGA.RGBImageData = (BaseColor[])Content_TGA.RGBImageData.Select(c => new BGRA8888Color(c.Red, c.Green, c.Blue, c.Alpha)).ToArray();
                                        } else {
                                            Content_TGA.RGBImageData = (BaseColor[])Content_TGA.RGBImageData.Select(c => new RGBA8888Color(c.Red, c.Green, c.Blue, c.Alpha)).ToArray();
                                        }
                                        break;
                                }
                            }

                            Content_TGA = s.SerializeObject<TGA>(Content_TGA, x =>
                            {
                                x.Pre_ColorOrder = colorOrder;
                                x.Pre_SkipHeader = true;
                                x.Header = new TGA_Header
                                {
                                    HasColorMap = false,
                                    ImageType = TGA_ImageType.UnmappedRGB,
                                    Width = Width,
                                    Height = Height,
                                    BitsPerPixel = (byte)(Format == TexColorFormat.BPP_24 ? 24 : 32),
                                };
                            }, name: nameof(Content_TGA));
                            hasReadContent = true;

                            if (!Loader.IsBinaryData && s.CurrentAbsoluteOffset + 26 + 0x20 == Offset.AbsoluteOffset + FileSize) {
                                // TGAs can have an optional 26 byte footer
                                Content_TGA.SerializeFooter(s);
                            }
						}
                        break;

                    case TexFileType.DDS:
                        if (IsContent || !Loader.IsBinaryData) 
                        {
                            if (ContentSize > 0) 
                            {
                                if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) 
                                {
                                    Content_Xenon = s.SerializeObject<TEX_Content_Xenon>(Content_Xenon, onPreSerialize: c => c.FileSize = ContentSize, name: nameof(Content_Xenon));
                                }
                                else 
                                { 
                                    // Serialize the header first, then set a custom one for the DDS struct based on the Jade properties instead
                                    Content_DDS_Header = s.SerializeObject<DDS_Header>(Content_DDS_Header, name: nameof(Content_DDS_Header));
                                    Content_DDS = s.SerializeObject<DDS>(Content_DDS, x =>
                                    {
                                        x.Pre_SkipHeader = true;
                                        x.Header = new DDS_Header
                                        {
                                            Flags = DDS_Header.DDS_HeaderFlags.DDS_HEADER_FLAGS_TEXTURE,
                                            Height = Height,
                                            Width = Width,
                                            PixelFormat = new DDS_PixelFormat
                                            {
                                                Flags = DDS_PixelFormat.DDS_PixelFormatFlags.DDPF_FOURCC,
                                                FourCC = Content_DDS_Header.PixelFormat.FourCC,
                                                RGBBitCount = 32,
                                            },
                                        };
                                    }, name: nameof(Content_DDS));
                                }
                                hasReadContent = true;
                            }
                        }
                        break;
                    case TexFileType.Raw:
                    case TexFileType.Jpeg:
                    case TexFileType.Bmp:
                    default:
                        if (IsContent || !Loader.IsBinaryData) {
                            Content = s.SerializeArray<byte>(Content, ContentSize, name: nameof(Content));
                            if (Content.Length > 0) 
                                hasReadContent = true;
                        }
                        break;
                }
                if (hasReadContent && (Flags & 0x40) != 0 && CanHaveFontDesc && !FontDesc.IsNull) {
                    TEX_GlobalList lists = Context.GetStoredObject<TEX_GlobalList>(Jade_BaseManager.TextureListKey);
                    var keyForTexture = Info?.ContentKey ?? ContentKey ?? Key;
                    //s.Log($"FONTDESC Key: {keyForTexture}");
                    if (!lists.FontDescriptors.ContainsKey(keyForTexture)
                        || (s.GetR1Settings().EngineVersion == EngineVersion.Jade_BGE && s.GetR1Settings().Platform == Platform.PC)) {
                        FontDesc?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
                        lists.FontDescriptors[keyForTexture] = FontDesc;
                    }
                }
                if (!Loader.IsBinaryData) s.Goto(s.CurrentPointer + 0x20);
            }
        }

		protected override void OnChangeContext(Context oldContext, Context newContext) {
			base.OnChangeContext(oldContext, newContext);
            if ((newContext.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR || newContext.GetR1Settings().EngineVersion == EngineVersion.Jade_RRRPrototype)
                && newContext.GetR1Settings().Platform == Platform.PC) {
                ConvertToTGA(oldContext, newContext);
            }
        }

		public void ConvertToTGA(Context oldContext, Context newContext) {
            var fileFormat = Info?.Type ?? Type;
            bool convert = false;
            if (oldContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && 
                !newContext.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Montreal_Key != null) {
                FileSize -= 4;
            }
            if (Content_RawPal != null) {
                if (oldContext.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) &&
                    !newContext.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon)) {
                    if (ContentSize >= 0x30) ContentSize = 0x30;
                    FileSize = 32 + ContentSize;
                }
            }
            switch (fileFormat) {
                case TexFileType.DDS:
                case TexFileType.JTX:
                    convert = true;
                    break;
            }
            if (convert) {
                if (Content_Xenon != null || Content_DDS != null || (Content_JTX != null && IsContent)) {
                    Texture2D tex = ToTexture2D();
                    if (tex.width > 512 || tex.height > 512) {
                        tex.ResizeImageData(tex.width / 4, tex.height / 4, mipmap: false, filter: FilterMode.Bilinear);
                    }
                    var pixels = tex.GetPixels();
                    Width = (ushort)tex.width;
                    Height = (ushort)tex.height;
                    /*if (Content_DDS != null) {
                        // invert y
                        Color[] newPixels = new Color[pixels.Length];
                        for (int y = 0; y < Height; y++) {
                            for (int x = 0; x < Width; x++) {
                                newPixels[y * Width + x] = pixels[(Height - 1 - y) * Width + x];
                            }
                        }
                        pixels = newPixels;
                    }*/
                    BaseColor[] pixelsBaseColor = null;
                    uint size = (uint)pixels.Length;

                    switch (Format) {
                        case TexColorFormat.BPP_24:
                            pixelsBaseColor = (BaseColor[])pixels.Select(c => new BGR888Color(c.r, c.g, c.b)).ToArray();
                            size *= 3;
                            break;
                        case TexColorFormat.BPP_32:
                        default:
                            pixelsBaseColor = (BaseColor[])pixels.Select(c => new BGRA8888Color(c.r, c.g, c.b, c.a)).ToArray();
                            size *= 4;
                            break;
                    } 
                    Content_TGA = new TGA() {
                        Pre_ColorOrder = TGA.RGBColorOrder.BGR,
                        Pre_SkipHeader = true,
                        Header = new TGA_Header {
                            HasColorMap = false,
                            ImageType = TGA_ImageType.UnmappedRGB,
                            Width = Width,
                            Height = Height,
                            BitsPerPixel = (byte)(Format == TexColorFormat.BPP_24 ? 24 : 32)                            
                        },
                        RGBImageData = pixelsBaseColor,
                    };
                    Content_TGA_Header = Content_TGA.Header;

                    Type = TexFileType.Tga;
                    if(Info != null) Info.Type = TexFileType.Tga;
                    FileSize = size
                        + 18 // Tga header size
                        + 32; // Jade header size
                } else {
                    Type = TexFileType.Tga;
                    if (Info != null) Info.Type = TexFileType.Tga;
                    if (Content_DDS != null || Content_JTX != null || Content_Xenon != null) FileSize = 32;
                }
            }
        }

        public Texture2D ToTexture2D()
        {
            var fileFormat = Info?.Type ?? Type;
            return fileFormat switch
            {
                TexFileType.Raw => null, // Gets parsed from RawPal
                TexFileType.SpriteGen => null, // Points to a RawPal
                TexFileType.Procedural => null, // Points to nothing
                TexFileType.Animated => null, // Points to various frames
                TexFileType.JTX => Content_JTX != null ? Content_JTX.ToTexture2D() : null,
                TexFileType.RawPal => (Info != null ? Info : this).Content_RawPal.PreferredSlot?.ToTexture2D(this),
                TexFileType.Tga => Content_TGA.ToTexture2D(),
                TexFileType.Jpeg => ToTexture2DFromJpeg(),
                TexFileType.DDS => Content_DDS != null ? Content_DDS.PrimaryTexture?.ToTexture2D(invertY: true) : Content_Xenon.ToTexture2D(),
                TexFileType.Bmp when (Content == null || Content.Length == 0) => null,
                _ => throw new NotImplementedException($"TODO: Implement texture type {fileFormat}")
            };
        }

        public Texture2D ToTexture2DFromJpeg()
        {
            var tex = TextureHelpers.CreateTexture2D(Width, Height);

            tex.LoadImage(Content);

            return tex;
        }

        public enum TexFileType : byte
        {
            Tga = 1,
            Bmp = 2,
            Jpeg = 3,
            SpriteGen = 4,
            Procedural = 5,
            Raw = 6,
            RawPal = 7,
            Animated = 9,
            JTX = 10,
            DDS = 11,
        }

        public enum TexColorFormat : byte
        {
            BPP_4 = 0x50,
            BPP_8 = 0x40,

            BPP_16 = 0x31,
            BPP_24 = 0x20,
            BPP_32 = 0x10,
        }

		protected override void OnChangedIsBinaryData() {
			base.OnChangedIsBinaryData();
            var sizeDiff = 0;
            if (Content_Animated != null) sizeDiff += Content_Animated.EditorSizeDifference;
            if (Content_Procedural != null) sizeDiff += Content_Procedural.EditorSizeDifference;

            if (CurrentIsBinaryData == true) {
                FileSize += (uint)sizeDiff;
            } else if (CurrentIsBinaryData == false) {
                FileSize -= (uint)sizeDiff;
            }
		}
	}
}