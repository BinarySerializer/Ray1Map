using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class TEX_Content_RawPal : BinarySerializable {
        public TEX_File Texture { get; set; }

        public TexPaletteReference[] References { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (!(Texture.FileSize > 0x50 || Texture.FileSize % 4 != 0)) {
                var count = (Texture.FileSize - (s.CurrentPointer - Texture.Offset)) / 12;
                References = s.SerializeObjectArray<TexPaletteReference>(References, count, name: nameof(References));
            }
        }

		public class TexPaletteReference : BinarySerializable {
            public Jade_TextureReference RawTexture { get; set; }
            public Jade_PaletteReference Palette { get; set; }
            public Jade_Key Unknown { get; set; }
            public override void SerializeImpl(SerializerObject s) {
				RawTexture = s.SerializeObject<Jade_TextureReference>(RawTexture, name: nameof(RawTexture))?.Resolve();
                Palette = s.SerializeObject<Jade_PaletteReference>(Palette, name: nameof(Palette))?.Resolve();
                Unknown = s.SerializeObject<Jade_Key>(Unknown, name: nameof(Unknown));
            }
		}
	}
}