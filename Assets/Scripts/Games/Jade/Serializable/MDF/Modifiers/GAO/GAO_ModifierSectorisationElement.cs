using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierSectorisationElement : MDF_Modifier {
		public uint Version { get; set; }
		public SectoElementType Type { get; set; }
		public uint Group1 { get; set; }
		public uint Group2 { get; set; }
		public uint Flags { get; set; }
		public Jade_Reference<OBJ_World_GroupObjectList> Group { get; set; }
		public uint MaxDepthToGroup1 { get; set; }
		public uint MaxDepthToGroup2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 1) {
				Type = s.Serialize<SectoElementType>(Type, name: nameof(Type));
				if (Version < 3 || !s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
					Group1 = s.Serialize<uint>(Group1, name: nameof(Group1));
					Group2 = s.Serialize<uint>(Group2, name: nameof(Group2));
				}
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				if (Type == SectoElementType.Sector && (Version == 2 || (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP) && Version >= 2))) {
					Group = s.SerializeObject<Jade_Reference<OBJ_World_GroupObjectList>>(Group, name: nameof(Group))
						?.Resolve(onPreSerialize: (_, gol) => gol.ResolveObjects =
						(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CloudyWithAChanceOfMeatballs) ? false : true),
						flags: LOA_Loader.ReferenceFlags.DontCache | LOA_Loader.ReferenceFlags.MustExist);
				}
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
					if (Type == SectoElementType.Door && Version >= 3) {
						Group1 = s.Serialize<uint>(Group1, name: nameof(Group1));
						Group2 = s.Serialize<uint>(Group2, name: nameof(Group2));
					}
					if (Type == SectoElementType.Door && Version >= 4) {
						MaxDepthToGroup1 = s.Serialize<uint>(MaxDepthToGroup1, name: nameof(MaxDepthToGroup1));
						MaxDepthToGroup2 = s.Serialize<uint>(MaxDepthToGroup2, name: nameof(MaxDepthToGroup2));
					}
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
						throw new BinarySerializableException(this, $"TODO for PoP:TFS: Implement LIGHT_Partition reference etc");
					}
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
