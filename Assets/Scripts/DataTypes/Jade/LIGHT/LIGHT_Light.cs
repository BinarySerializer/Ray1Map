using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_Light : GRO_GraphicRenderObject {
		public uint Flags { get; set; }
		public Jade_Color Color { get; set; }
		public LIGHT_XenonData1 XenonData1 { get; set; }
		public float AddMaterial { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public LIGHT_XenonData2 XenonData2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
			if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon && BitHelpers.ExtractBits((int)Flags, 3, 0) == 7) {
				XenonData1 = s.SerializeObject<LIGHT_XenonData1>(XenonData1, name: nameof(XenonData1));
			}
			AddMaterial = s.Serialize<float>(AddMaterial, name: nameof(AddMaterial));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			if ((XenonData1 != null && BitHelpers.ExtractBits((int)XenonData1.LightFlags, 3, 0) == 5)
				|| (XenonData1 == null && s.GetR1Settings().Jade_Version == Jade_Version.Xenon && BitHelpers.ExtractBits((int)Flags, 3, 0) == 5)) {
				XenonData2 = s.SerializeObject<LIGHT_XenonData2>(XenonData2, name: nameof(XenonData2));
			}
		}
	}
}
