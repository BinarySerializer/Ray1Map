using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class DARE_InaudibleSector : Jade_File {
		public Reference[] WorldKeys { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			var count = FileSize >> (Loader.IsBinaryData ? 2 : 3);
			WorldKeys = s.SerializeObjectArray<Reference>(WorldKeys, count, name: nameof(WorldKeys));
		}

		public class Reference : BinarySerializable {
			public Jade_Reference<WOR_World> WorldKey { get; set; }
			public uint UInt_Editor { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				WorldKey = s.SerializeObject<Jade_Reference<WOR_World>>(WorldKey, name: nameof(WorldKey));
				if(!Loader.IsBinaryData) UInt_Editor = s.Serialize<uint>(UInt_Editor, name: nameof(UInt_Editor));
			}
		}
	}
}
