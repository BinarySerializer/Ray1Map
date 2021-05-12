using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Base : BinarySerializable {
		public uint Type { get; set; }
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public OBJ_GameObject_Visual Visual { get; set; } // GameObjectAnim in Montpellier versions, Visu in Montreal
		public OBJ_GameObject_Hierarchy HierarchyData { get; set; }
		public OBJ_GameObject_ActionData ActionData { get; set; }
		public DYN_ODE ODE { get; set; }
		public OBJ_GameObject_AdditionalMatrix AddMatrix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasGameObjectAnim)) {
				Visual = s.SerializeObject<OBJ_GameObject_Visual>(Visual, onPreSerialize: o => o.Version = Type, name: nameof(Visual));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasHierarchy)) {
				HierarchyData = s.SerializeObject<OBJ_GameObject_Hierarchy>(HierarchyData, name: nameof(HierarchyData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasActionData)) {
				ActionData = s.SerializeObject<OBJ_GameObject_ActionData>(ActionData, name: nameof(ActionData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasDynamics)) {
				ODE = s.SerializeObject<DYN_ODE>(ODE, name: nameof(ODE));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasAddMatrix)) {
				AddMatrix = s.SerializeObject<OBJ_GameObject_AdditionalMatrix>(AddMatrix, onPreSerialize: vu2 => vu2.FlagsIdentity = FlagsIdentity, name: nameof(AddMatrix));
			}
		}
	}
}
