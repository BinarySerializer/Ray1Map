using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_AdditionalMatrix : BinarySerializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public uint Count { get; set; }
		public GizmoPtr[] Gizmos { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			Gizmos = s.SerializeObjectArray<GizmoPtr>(Gizmos, Count, onPreSerialize: u => u.FlagsIdentity = FlagsIdentity, name: nameof(Gizmos));
		}
		public class GizmoPtr : BinarySerializable {
			public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // set in onPreSerialize

			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
			public uint MatrixID { get; set; }

			public Jade_Matrix Matrix { get; set; }
			public string Name { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.AddMatArePointer)) {
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
					MatrixID = s.Serialize<uint>(MatrixID, name: nameof(MatrixID));
				} else {
					LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

					Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
					if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				}
			}
		}
	}
}
