using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class WOR_Secto : BinarySerializable {
		public WOR_Secto_Flags Flags { get; set; }
		public byte[] RefVis { get; set; }
		public byte[] RefAct { get; set; }
		public string Name { get; set; }
		public WOR_Portal[] Portals { get; set; }
		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			Flags = s.Serialize<WOR_Secto_Flags>(Flags, name: nameof(Flags));
			RefVis = s.SerializeArray<byte>(RefVis, 0x10, name: nameof(RefVis));
			RefAct = s.SerializeArray<byte>(RefAct, 0x10, name: nameof(RefAct));
			if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			Portals = s.SerializeObjectArray<WOR_Portal>(Portals, 16, name: nameof(Portals));
		}
	}
}
