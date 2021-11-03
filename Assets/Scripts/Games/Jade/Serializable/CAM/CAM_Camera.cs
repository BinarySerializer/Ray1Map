using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in CAM_p_CreateFromBuffer
	public class CAM_Camera : GRO_GraphicRenderObject {
		public uint Flags { get; set; }
		public float NearPlane { get; set; }
		public float FarPlane { get; set; }
		public float FieldOfVision { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			NearPlane = s.Serialize<float>(NearPlane, name: nameof(NearPlane));
			FarPlane = s.Serialize<float>(FarPlane, name: nameof(FarPlane));
			FieldOfVision = s.Serialize<float>(FieldOfVision, name: nameof(FieldOfVision));
		}
	}
}
