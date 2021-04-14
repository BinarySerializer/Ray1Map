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
                UsedReference?.Resolve();
            }
        }

        public TexPaletteReference UsedReference {
            get {
                var palOrder = GetPaletteOrder(Context.GetR1Settings());
                for (int i = 0; i < palOrder.Length; i++) {
                    var pi = palOrder[i];
                    if (pi < 0) break;
                    if (pi >= References.Length) continue;
                    var reference = References[pi];
                    if (!reference.RawTexture.IsNull || !reference.Palette.IsNull || !reference.Unknown.IsNull) {
                        return reference;
                    }
                }
                return null;
            }
        }

        public static sbyte[] GetPaletteOrder(GameSettings settings) {
            switch (settings.GameModeSelection) {
                case GameModeSelection.RaymanRavingRabbidsWii:
                    return PaletteOrderWii;
                case GameModeSelection.RaymanRavingRabbidsXbox360:
                    return PaletteOrderXbox360;
                default:
                    return PaletteOrderPC;
            }
        }
        public static sbyte[] PaletteOrderWii = new sbyte[] { 1, 0, -1, -1 };
        public static sbyte[] PaletteOrderPC = new sbyte[] { 0, -1, -1, -1 };
        public static sbyte[] PaletteOrderXbox360 = new sbyte[] { 0, 2, -1, -1 };

        public class TexPaletteReference : BinarySerializable {
            public Jade_TextureReference RawTexture { get; set; }
            public Jade_PaletteReference Palette { get; set; }
            public Jade_TextureReference Unknown { get; set; }
            public override void SerializeImpl(SerializerObject s) {
				RawTexture = s.SerializeObject<Jade_TextureReference>(RawTexture, name: nameof(RawTexture));
                Palette = s.SerializeObject<Jade_PaletteReference>(Palette, name: nameof(Palette));
                Unknown = s.SerializeObject<Jade_TextureReference>(Unknown, name: nameof(Unknown));
            }

            public bool HasTexture => RawTexture?.Info != null || Palette?.Value != null || Unknown?.Info != null;

            public Jade_TextureReference TextureRef {
                get {
                    if (RawTexture.IsNull && Palette.IsNull) {
                        return Unknown;
                    } else {
                        return RawTexture;
                    }
                }
            }

            public void Resolve() {
                if (RawTexture.IsNull && Palette.IsNull) {
                    Unknown?.Resolve();
                } else {//if (Unknown.IsNull) {
                    RawTexture?.Resolve();
                    Palette?.Resolve();
                } /*else {
                    //throw new NotImplementedException("TODO: Implement RawPal textures where 
                }*/
            }
            public Texture2D ToTexture2D(TEX_File contentFile)
            {
                var texture = Context.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360 ? contentFile : RawTexture.Content;
                var pal = Palette.Value;

                if (texture == null || pal == null)
                    return null;

                if (texture.ColorFormat != TEX_File.TexColorFormat.BPP_4 && texture.ColorFormat != TEX_File.TexColorFormat.BPP_8)
                    throw new NotImplementedException($"Unsupported raw texture format {texture.ColorFormat}");

                var tex = TextureHelpers.CreateTexture2D(texture.Width, texture.Height);

                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        if (texture.ColorFormat == TEX_File.TexColorFormat.BPP_8)
                            tex.SetPixel(x, y, pal.Colors[texture.Content[(y * texture.Width + x)]].GetColor());
                        else
                            tex.SetPixel(x, y, pal.Colors[BitHelpers.ExtractBits(texture.Content[(y * texture.Width + x) / 2], 4, x % 2 == 1 ? 0 : 4)].GetColor());
                    }
                }

                tex.Apply();
                return tex;
            }
		}
	}
}