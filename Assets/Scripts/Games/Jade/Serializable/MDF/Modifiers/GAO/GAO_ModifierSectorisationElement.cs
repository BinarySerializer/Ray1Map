using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierSectorisationElement : MDF_Modifier {
		public uint Version { get; set; }
		public SectoElementType Type { get; set; }
		public uint Group1 { get; set; }
		public uint Group2 { get; set; }
		public uint Flags { get; set; }
		public Jade_Reference<OBJ_World_GroupObjectList> Group { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				Type = s.Serialize<SectoElementType>(Type, name: nameof(Type));
				Group1 = s.Serialize<uint>(Group1, name: nameof(Group1));
				Group2 = s.Serialize<uint>(Group2, name: nameof(Group2));
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				if (Type == SectoElementType.Sector && Version >= 2) {
					Group = s.SerializeObject<Jade_Reference<OBJ_World_GroupObjectList>>(Group, name: nameof(Group))
						?.Resolve(onPreSerialize: (_, gol) => gol.ResolveObjects =
						(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs) ? false : true),
						flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.Log);
				}
			}
		}

		public enum SectoElementType : uint {
			Sector = 0,
			Door = 1,
			Occluder = 2
		}
	}
}
