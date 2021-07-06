using BinarySerializer;
using System;
using BinarySerializer.Image;
using UnityEngine;

namespace R1Engine.Jade
{
    // Used to test if an unknown Jade_File is a texture in the Montreal games (a hack so we can open WOWs separately)
    public class TEX_File_Montreal_Dummy : Jade_File {
        public Jade_Key Montreal_Key { get; set; }
        public int Mark { get; set; } // Always 0xFFFFFFFF in files
        public ushort Flags { get; set; }
        public TEX_File.TexFileType Type { get; set; }
        public TEX_File.TexColorFormat Format { get; set; } // Determines bits per pixel
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint Color { get; set; }
        public Jade_Reference<STR_FontDescriptor> FontDesc { get; set; }
        public Jade_Code CodeCado1234 { get; set; } // Usually CAD01234
        public Jade_Code CodeFF00FF { get; set; } // Checked for 0xFF00FF
        public Jade_Code CodeCode { get; set; } // Checked for 0xC0DEC0DE

        public uint ContentSize { get; set; }
        public byte[] Content { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (FileSize >= 32 && (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) || !Loader.IsBinaryData || FileSize >= 36)) {
                if (!Loader.IsBinaryData)
                    s.Goto(s.CurrentPointer + FileSize - 32);

                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Loader.IsBinaryData)
                    Montreal_Key = s.SerializeObject<Jade_Key>(Montreal_Key, name: nameof(Montreal_Key));
                Mark = s.Serialize<int>(Mark, name: nameof(Mark));
                if (Mark == -1) {
                    Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
                    Type = s.Serialize<TEX_File.TexFileType>(Type, name: nameof(Type));
                    Format = s.Serialize<TEX_File.TexColorFormat>(Format, name: nameof(Format));
                    Width = s.Serialize<ushort>(Width, name: nameof(Width));
                    Height = s.Serialize<ushort>(Height, name: nameof(Height));
                    Color = s.Serialize<uint>(Color, name: nameof(Color));
                    FontDesc = s.SerializeObject<Jade_Reference<STR_FontDescriptor>>(FontDesc, name: nameof(FontDesc));
                    CodeCado1234 = s.Serialize<Jade_Code>(CodeCado1234, name: nameof(CodeCado1234));
                    CodeFF00FF = s.Serialize<Jade_Code>(CodeFF00FF, name: nameof(CodeFF00FF));
                    CodeCode = s.Serialize<Jade_Code>(CodeCode, name: nameof(CodeCode));
                }
            }
            s.Goto(Offset + FileSize);
        }

        public bool IsTexture =>
            CodeCado1234 == Jade_Code.CAD01234 && CodeFF00FF == Jade_Code.FF00FF && CodeCode == Jade_Code.CodeCode &&
            Mark == -1 && (!Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) || Montreal_Key == Key);
	}
}