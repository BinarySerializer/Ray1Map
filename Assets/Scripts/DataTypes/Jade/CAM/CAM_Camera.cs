using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in CAM_p_CreateFromBuffer
	public class CAM_Camera : GRO_GraphicRenderObject {
		public uint UInt_00 { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
		}
	}
}
