using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSNAKE : MDF_Modifier {
		public uint Version { get; set; }
		public uint BonesCount { get; set; }
		public float Inertie { get; set; }
		public float BlendDist { get; set; }
		public float Attenuation { get; set; }
		public int[] Bones { get; set; }
		public uint Flags { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (s.GetR1Settings().EngineVersion >= EngineVersion.Jade_KingKong || !Loader.IsBinaryData) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			}
			BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
			Inertie = s.Serialize<float>(Inertie, name: nameof(Inertie));
			BlendDist = s.Serialize<float>(BlendDist, name: nameof(BlendDist));
			Attenuation = s.Serialize<float>(Attenuation, name: nameof(Attenuation));
			Bones = s.SerializeArray<int>(Bones, 20, name: nameof(Bones));

			if (Version >= 9) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
		}
	}
}
