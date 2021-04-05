using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_SKN_Load
	public class GEO_GeometricObject_SKN : BinarySerializable {
		public short RootBoneID { get; set; }
		public ushort BonesCount { get; set; }
		public Bone[] Bones { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RootBoneID = s.Serialize<short>(RootBoneID, name: nameof(RootBoneID));
			BonesCount = s.Serialize<ushort>(BonesCount, name: nameof(BonesCount));
			Bones = s.SerializeObjectArray<Bone>(Bones, BonesCount, name: nameof(Bones));
		}

		public class Bone : BinarySerializable {
			public short BoneID { get; set; }
			public ushort WeightsCount { get; set; }
			public Jade_Matrix BindPose { get; set; }
			public float[] Weights { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BoneID = s.Serialize<short>(BoneID, name: nameof(BoneID));
				WeightsCount = s.Serialize<ushort>(WeightsCount, name: nameof(WeightsCount));
				BindPose = s.SerializeObject<Jade_Matrix>(BindPose, name: nameof(BindPose));
				Weights = s.SerializeArray<float>(Weights, WeightsCount, name: nameof(Weights));
			}
		}
	}
}
