using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_VisualUnknown2Data : BinarySerializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		public uint Unk2_Count { get; set; }
		public Unk2[] Unk2s { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Unk2_Count = s.Serialize<uint>(Unk2_Count, name: nameof(Unk2_Count));
			Unk2s = s.SerializeObjectArray<Unk2>(Unk2s, Unk2_Count, onPreSerialize: u => u.FlagsIdentity = FlagsIdentity, name: nameof(Unk2s));
		}
		public class Unk2 : BinarySerializable {
			public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // set in onPreSerialize

			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
			public uint GameObject_Unk { get; set; }

			public Jade_Matrix Matrix { get; set; }
			public string Name { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag24)) {
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
					GameObject_Unk = s.Serialize<uint>(GameObject_Unk, name: nameof(GameObject_Unk));
				} else {
					LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

					Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
					if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				}
			}
		}
	}
}
