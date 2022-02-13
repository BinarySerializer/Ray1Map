using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_Base : BinarySerializable {
		public OBJ_GameObject GameObject { get; set; } // Set in OnPreSerialize
		public OBJ_GameObject_IdentityFlags FlagsIdentity => GameObject.FlagsIdentity;

		public OBJ_GameObject_Visual Visual { get; set; } // GameObjectAnim in Montpellier versions, Visu in Montreal
		public OBJ_GameObject_Hierarchy HierarchyData { get; set; }
		public OBJ_GameObject_ActionData ActionData { get; set; }
		public DYN_ODE ODE { get; set; }
		public OBJ_GameObject_AdditionalMatrix AddMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Visu)) {
				Visual = s.SerializeObject<OBJ_GameObject_Visual>(Visual, onPreSerialize: o => o.Version = GameObject.Version, name: nameof(Visual));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Hierarchy)) {
				HierarchyData = s.SerializeObject<OBJ_GameObject_Hierarchy>(HierarchyData, name: nameof(HierarchyData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Anims)) {
				ActionData = s.SerializeObject<OBJ_GameObject_ActionData>(ActionData, o => o.Base = this, name: nameof(ActionData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.ODE) && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				ODE = s.SerializeObject<DYN_ODE>(ODE, name: nameof(ODE));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.AdditionalMatrix)) {
				AddMatrix = s.SerializeObject<OBJ_GameObject_AdditionalMatrix>(AddMatrix, onPreSerialize: vu2 => vu2.FlagsIdentity = FlagsIdentity, name: nameof(AddMatrix));
			}
		}
	}
}
