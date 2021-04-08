using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in LIGHT_p_CreateFromBuffer
	public class LIGHT_Light : GRO_GraphicRenderObject {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public LIGHT_XenonData1 XenonData1 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public float Float_14 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360 && BitHelpers.ExtractBits((int)UInt_00, 3, 0) == 7) {
				XenonData1 = s.SerializeObject<LIGHT_XenonData1>(XenonData1, name: nameof(XenonData1));
			}
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			if ((XenonData1 != null && BitHelpers.ExtractBits((int)XenonData1.LightFlags, 3, 0) == 5)
				|| (XenonData1 == null && s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360 && BitHelpers.ExtractBits((int)UInt_00, 3, 0) == 5)) {
				throw new NotImplementedException($"TODO: Light X360 stuff");
			}
		}
	}
}
