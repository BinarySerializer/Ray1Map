﻿using BinarySerializer;

namespace Ray1Map.Jade {
	// MAT_pst_CreateSingleFromBuffer
	public class MAT_SIN_SingleMaterial : GRO_GraphicRenderObject
    {
        public SerializableColor Ambiant { get; set; } = SerializableColor.White;
        public SerializableColor Diffuse { get; set; } = SerializableColor.White;
        public SerializableColor Specular { get; set; } = SerializableColor.White; // Color?
        public uint SpecularExp { get; set; } // Read as a float, but it's 0x80000000, ie negative zero
		public float Opacity { get; set; }
		public uint Flags { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public uint ValidateMask { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Ambiant = s.SerializeInto<SerializableColor>(Ambiant, BitwiseColor.RGBA8888, name: nameof(Ambiant));
			Diffuse = s.SerializeInto<SerializableColor>(Diffuse, BitwiseColor.RGBA8888, name: nameof(Diffuse));
			Specular = s.SerializeInto<SerializableColor>(Specular, BitwiseColor.RGBA8888, name: nameof(Specular));
			SpecularExp = s.Serialize<uint>(SpecularExp, name: nameof(SpecularExp));
			Opacity = s.Serialize<float>(Opacity, name: nameof(Opacity));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
			ValidateMask = s.Serialize<uint>(ValidateMask, name: nameof(ValidateMask));

			Texture?.Resolve(s, RRR2_readBool: true);
		}
	}
}
