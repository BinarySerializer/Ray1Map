using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_ModifierSymetrie : MDF_Modifier {
		public uint UInt_00_Editor { get; set; }
		public uint Flags { get; set; }
		public float SymetrieOffset { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_00_Editor = s.Serialize<uint>(UInt_00_Editor, name: nameof(UInt_00_Editor));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			SymetrieOffset = s.Serialize<float>(SymetrieOffset, name: nameof(SymetrieOffset));
		}
	}
}
