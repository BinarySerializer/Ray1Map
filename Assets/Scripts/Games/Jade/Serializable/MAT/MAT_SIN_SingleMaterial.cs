using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// MAT_pst_CreateSingleFromBuffer
	public class MAT_SIN_SingleMaterial : GRO_GraphicRenderObject {
		public Jade_Color Ambiant { get; set; }
		public Jade_Color Diffuse { get; set; }
		public Jade_Color Specular { get; set; } // Color?
		public uint SpecularExp { get; set; } // Read as a float, but it's 0x80000000, ie negative zero
		public float Opacity { get; set; }
		public uint Flags { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public uint ValidateMask { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Ambiant = s.SerializeObject<Jade_Color>(Ambiant, name: nameof(Ambiant));
			Diffuse = s.SerializeObject<Jade_Color>(Diffuse, name: nameof(Diffuse));
			Specular = s.SerializeObject<Jade_Color>(Specular, name: nameof(Specular));
			SpecularExp = s.Serialize<uint>(SpecularExp, name: nameof(SpecularExp));
			Opacity = s.Serialize<float>(Opacity, name: nameof(Opacity));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
			ValidateMask = s.Serialize<uint>(ValidateMask, name: nameof(ValidateMask));

			Texture?.Resolve(s, RRR2_readBool: true);
		}
	}
}
