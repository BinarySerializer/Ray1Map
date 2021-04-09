using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_RawPal : BinarySerializable {
        public TEX_File Texture { get; set; }

        public TexPaletteReference[] References { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (!(Texture.FileSize > 0x50 || Texture.FileSize % 4 != 0)) {
                var count = (Texture.FileSize - (s.CurrentPointer - Texture.Offset)) / 12;
                References = s.SerializeObjectArray<TexPaletteReference>(References, count, name: nameof(References));
                foreach (var reference in References) {
                    if (!reference.RawTexture.IsNull || !reference.Palette.IsNull || !reference.Unknown.IsNull) {
                        reference.Resolve();
                        break;
                    }
                }
            }
        }

		public class TexPaletteReference : BinarySerializable {
            public Jade_TextureReference RawTexture { get; set; }
            public Jade_PaletteReference Palette { get; set; }
            public Jade_Key Unknown { get; set; }
            public override void SerializeImpl(SerializerObject s) {
				RawTexture = s.SerializeObject<Jade_TextureReference>(RawTexture, name: nameof(RawTexture));
                Palette = s.SerializeObject<Jade_PaletteReference>(Palette, name: nameof(Palette));
                Unknown = s.SerializeObject<Jade_Key>(Unknown, name: nameof(Unknown));
            }

            public void Resolve() {
                RawTexture?.Resolve();
                Palette?.Resolve();
            }
            public Texture2D ToTexture2D()
            {
                var texture = RawTexture.Content;
                var pal = Palette.Value;

                if (texture == null || pal == null)
                    return null;

                if (texture.ColorFormat != TEX_File.TexColorFormat.BPP_4&& texture.ColorFormat != TEX_File.TexColorFormat.BPP_8)
                    throw new NotImplementedException($"Unsupported raw texture format {texture.ColorFormat}");

                var tex = TextureHelpers.CreateTexture2D(texture.Width, texture.Height);

                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        if (texture.ColorFormat == TEX_File.TexColorFormat.BPP_8)
                            tex.SetPixel(x, y, pal.Colors[texture.Content_Raw[(y * texture.Width + x)]].GetColor());
                        else
                            tex.SetPixel(x, y, pal.Colors[BitHelpers.ExtractBits(texture.Content_Raw[(y * texture.Width + x) / 2], 4, x % 2 == 1 ? 0 : 4)].GetColor());
                    }
                }

                tex.Apply();
                return tex;
            }
		}
	}
}