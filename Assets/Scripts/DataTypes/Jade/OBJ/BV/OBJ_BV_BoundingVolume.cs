using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_BV_BoundingVolume : BinarySerializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public uint UInt_00_Editor { get; set; }
		public OBJ_BV_Box AABB_Box { get; set; }
		public OBJ_BV_Box OB_Box { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
			AABB_Box = s.SerializeObject<OBJ_BV_Box>(AABB_Box, name: nameof(AABB_Box));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.HasOBBox)) {
				OB_Box = s.SerializeObject<OBJ_BV_Box>(OB_Box, name: nameof(OB_Box));
			}
		}
	}
}
