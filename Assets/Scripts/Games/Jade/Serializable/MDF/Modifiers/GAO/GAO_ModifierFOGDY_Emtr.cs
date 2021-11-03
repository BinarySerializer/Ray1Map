using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierFOGDY_Emtr : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public uint ActiveBoneNumber { get; set; }
		public Bone[] Bones { get; set; }
		public uint ActiveChannel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			ActiveBoneNumber = s.Serialize<uint>(ActiveBoneNumber, name: nameof(ActiveBoneNumber));
			Bones = s.SerializeObjectArray<Bone>(Bones, 8, onPreSerialize: e => e.Modifier = this, name: nameof(Bones));
			if(Version >= 1) ActiveChannel = s.Serialize<uint>(ActiveChannel, name: nameof(ActiveChannel));
		}

		public class Bone : BinarySerializable {
			public GAO_ModifierFOGDY_Emtr Modifier { get; set; }

			public uint BoneID { get; set; }
			public float BoneRadius { get; set; }
			public Jade_Vector BoneDelta { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				BoneID = s.Serialize<uint>(BoneID, name: nameof(BoneID));
				BoneRadius = s.Serialize<float>(BoneRadius, name: nameof(BoneRadius));
				if (Modifier.Version >= 2) BoneDelta = s.SerializeObject<Jade_Vector>(BoneDelta, name: nameof(BoneDelta));
			}
		}
	}
}
