using BinarySerializer;

namespace Ray1Map.Jade {
	public class MDF_Melt : MDF_Modifier {
		public uint Version { get; set; }
		public float VD_Strength { get; set; }
		public float LD_TotalTime { get; set; }
		public float LD_Strength { get; set; }
		public float ZD_InitSpeed { get; set; }
		public float ZD_Acceleration { get; set; }
		public float FinalThickness { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			VD_Strength = s.Serialize<float>(VD_Strength, name: nameof(VD_Strength));
			LD_TotalTime = s.Serialize<float>(LD_TotalTime, name: nameof(LD_TotalTime));
			LD_Strength = s.Serialize<float>(LD_Strength, name: nameof(LD_Strength));
			ZD_InitSpeed = s.Serialize<float>(ZD_InitSpeed, name: nameof(ZD_InitSpeed));
			ZD_Acceleration = s.Serialize<float>(ZD_Acceleration, name: nameof(ZD_Acceleration));
			FinalThickness = s.Serialize<float>(FinalThickness, name: nameof(FinalThickness));
		}
	}
}
