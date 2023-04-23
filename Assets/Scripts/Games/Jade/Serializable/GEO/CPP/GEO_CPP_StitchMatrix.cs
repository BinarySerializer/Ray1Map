using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class GEO_CPP_StitchMatrix : BinarySerializable {
		public Bone[] Bones { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Bones = s.SerializeObjectArray<Bone>(Bones, 3, name: nameof(Bones));
		}

		public class Bone : BinarySerializable {
			public uint BoneRef { get; set; }
			public float Weight { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				BoneRef = s.Serialize<uint>(BoneRef, name: nameof(BoneRef));
				Weight = s.Serialize<float>(Weight, name: nameof(Weight));
			}
		}
	}
}
