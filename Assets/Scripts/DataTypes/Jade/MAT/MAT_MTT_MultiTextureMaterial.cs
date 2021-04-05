using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// MAT_pst_CreateMultiTextureFromBuffer
	public class MAT_MTT_MultiTextureMaterial : GRO_GraphicRenderObject {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint UInt_08 { get; set; } // Color?
		public uint UInt_0C { get; set; } // Read as a float, but it's 0x80000000, ie negative zero
		public float Float_10 { get; set; }
		public uint UInt_14 { get; set; }
		public uint TexturePointer { get; set; } // Leftover, this is overwritten
		public uint UInt_1C { get; set; }

		// Float_0C Negative -> NF (Negative Float)
		public byte NF_Byte_00 { get; set; }
		public byte NF_Byte_01 { get; set; }
		public short NF_Short_02 { get; set; }

		public TextureDescriptor[] Textures { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
			TexturePointer = s.Serialize<uint>(TexturePointer, name: nameof(TexturePointer));
			UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));

			if (BitHelpers.ExtractBits((int)UInt_0C,1,31) == 1) {
				NF_Byte_00 = s.Serialize<byte>(NF_Byte_00, name: nameof(NF_Byte_00));
				NF_Byte_01 = s.Serialize<byte>(NF_Byte_01, name: nameof(NF_Byte_01));
				NF_Short_02 = s.Serialize<short>(NF_Short_02, name: nameof(NF_Short_02));
			}

			Textures = s.SerializeObjectArrayUntil<TextureDescriptor>(Textures, t => t.Short_00 == 0, includeLastObj: true, name: nameof(Textures));
		}

		public class TextureDescriptor : BinarySerializable {
			public short Short_00 { get; set; }
			public short Short_02 { get; set; }
			public uint UInt_04 { get; set; }
			public uint UInt_08 { get; set; }
			public uint Float_0C { get; set; }
			public Jade_TextureReference Texture { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
				Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
				UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
				UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
				Float_0C = s.Serialize<uint>(Float_0C, name: nameof(Float_0C));
				Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture))?.Resolve();
			}
		}
	}
}
