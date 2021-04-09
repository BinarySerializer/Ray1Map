using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_ModifierSnap : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public uint UnknownCount { get; set; }
		public Unk[] Unknown { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			UnknownCount = s.Serialize<uint>(UnknownCount, name: nameof(UnknownCount));
			Unknown = s.SerializeObjectArray<Unk>(Unknown, UnknownCount, name: nameof(Unknown));
		}

		public class Unk : BinarySerializable {
			public uint UInt_00 { get; set; }
			public uint UInt_04 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			}
		}
	}
}
