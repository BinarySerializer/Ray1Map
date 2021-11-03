using BinarySerializer;

namespace Ray1Map.Jade {
	public class DARE_SoundParam : BinarySerializable {
		public uint Version { get; set; }
		public uint Flag { get; set; }
		public float DistSatur { get; set; }
		public float DistBackGround { get; set; }
		public float DistTransitLinear { get; set; }
		public float DistInaudible { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flag = s.Serialize<uint>(Flag, name: nameof(Flag));
			DistSatur = s.Serialize<float>(DistSatur, name: nameof(DistSatur));
			DistBackGround = s.Serialize<float>(DistBackGround, name: nameof(DistBackGround));
			if (Version >= 16) {
				DistTransitLinear = s.Serialize<float>(DistTransitLinear, name: nameof(DistTransitLinear));
				DistInaudible = s.Serialize<float>(DistInaudible, name: nameof(DistInaudible));
			}
		}
	}
}
