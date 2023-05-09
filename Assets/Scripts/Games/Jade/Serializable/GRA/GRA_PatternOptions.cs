using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_PatternOptions : BinarySerializable {
		public uint Pre_ObjectVersion { get; set; }

		public uint RandomSeed { get; set; }
		public uint GrassLODCount { get; set; }
		public float RandomPositionFactor { get; set; }
		public float RandomWidthFactor { get; set; }
		public float RandomHeightFactor { get; set; }
		public LODInfo[] GrassLOD { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RandomSeed = s.Serialize<uint>(RandomSeed, name: nameof(RandomSeed));
			GrassLODCount = s.Serialize<uint>(GrassLODCount, name: nameof(GrassLODCount));
			if (Pre_ObjectVersion >= 3) {
				RandomPositionFactor = s.Serialize<float>(RandomPositionFactor, name: nameof(RandomPositionFactor));
				RandomWidthFactor = s.Serialize<float>(RandomWidthFactor, name: nameof(RandomWidthFactor));
				RandomHeightFactor = s.Serialize<float>(RandomHeightFactor, name: nameof(RandomHeightFactor));
			}
			GrassLOD = s.SerializeObjectArray<LODInfo>(GrassLOD, GrassLODCount, name: nameof(GrassLOD));
		}

		public class LODInfo : BinarySerializable {
			public float Distance { get; set; }
			public uint Density { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
				Density = s.Serialize<uint>(Density, name: nameof(Density));
			}
		}
	}
}
