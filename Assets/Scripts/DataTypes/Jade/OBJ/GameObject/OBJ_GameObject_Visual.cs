using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Visual : BinarySerializable {
		public uint Type { get; set; }
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public OBJ_GameObject_GeometricData GeometricData { get; set; }
		public OBJ_GameObject_HierarchyData HierarchyData { get; set; }
		public OBJ_GameObject_ActionData ActionData { get; set; }
		public OBJ_GameObject_VisualUnknownData VisualUnknownData { get; set; }
		public OBJ_GameObject_VisualUnknown2Data VisualUnknown2Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasGeometricData)) {
				GeometricData = s.SerializeObject<OBJ_GameObject_GeometricData>(GeometricData, onPreSerialize: o => o.Type = Type, name: nameof(GeometricData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasHierarchyData)) {
				HierarchyData = s.SerializeObject<OBJ_GameObject_HierarchyData>(HierarchyData, name: nameof(HierarchyData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasActionData)) {
				ActionData = s.SerializeObject<OBJ_GameObject_ActionData>(ActionData, name: nameof(ActionData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag28)) {
				VisualUnknownData = s.SerializeObject<OBJ_GameObject_VisualUnknownData>(VisualUnknownData, name: nameof(VisualUnknownData));
			}
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag21)) {
				VisualUnknown2Data = s.SerializeObject<OBJ_GameObject_VisualUnknown2Data>(VisualUnknown2Data, onPreSerialize: vu2 => vu2.FlagsIdentity = FlagsIdentity, name: nameof(VisualUnknown2Data));
			}
		}
	}
}
