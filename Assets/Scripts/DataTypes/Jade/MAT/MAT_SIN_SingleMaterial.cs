using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// MAT_pst_CreateSingleFromBuffer
	public class MAT_SIN_SingleMaterial : GRO_GraphicRenderObject {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; } // Color?
		public uint UInt_0C { get; set; } // Read as a float, but it's 0x80000000, ie negative zero
		public float Float_10 { get; set; }
		public uint UInt_14 { get; set; }
		public Jade_TextureReference Texture { get; set; }
		public uint UInt_1C { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
			Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture));
			UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));

			Texture?.Resolve(s, RRR2_readBool: true);
		}
	}
}
