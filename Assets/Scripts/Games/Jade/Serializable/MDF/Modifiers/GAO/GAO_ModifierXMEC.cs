using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierXMEC : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Version { get; set; }
		public float Distance { get; set; }
		public float Gravity { get; set; }
		public float Elasticity { get; set; }
		public float Ground { get; set; }
		public float GroundFriction { get; set; }
		public float Tension { get; set; }
		public uint Flags { get; set; }
		public uint BonesCount { get; set; }
		public int RefBone { get; set; }
		public int[] Bones { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
				Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
				Elasticity = s.Serialize<float>(Elasticity, name: nameof(Elasticity));
				Ground = s.Serialize<float>(Ground, name: nameof(Ground));
				GroundFriction = s.Serialize<float>(GroundFriction, name: nameof(GroundFriction));
				Tension = s.Serialize<float>(Tension, name: nameof(Tension));
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
				RefBone = s.Serialize<int>(RefBone, name: nameof(RefBone));
				Bones = s.SerializeArray<int>(Bones, 50, name: nameof(Bones));
			}
		}
	}
}
