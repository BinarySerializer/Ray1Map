using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class WOR_Portal : BinarySerializable {
		public WOR_Portal_Flags Flags { get; set; }
		public byte ShareSect { get; set; }
		public byte SharePortal { get; set; }
		public Jade_Vector vA { get; set; }
		public Jade_Vector vB { get; set; }
		public Jade_Vector vC { get; set; }
		public Jade_Vector vD { get; set; }
		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			Flags = s.Serialize<WOR_Portal_Flags>(Flags, name: nameof(Flags));
			ShareSect = s.Serialize<byte>(ShareSect, name: nameof(ShareSect));
			SharePortal = s.Serialize<byte>(SharePortal, name: nameof(SharePortal));
			vA = s.SerializeObject<Jade_Vector>(vA, name: nameof(vA));
			vB = s.SerializeObject<Jade_Vector>(vB, name: nameof(vB));
			vC = s.SerializeObject<Jade_Vector>(vC, name: nameof(vC));
			vD = s.SerializeObject<Jade_Vector>(vD, name: nameof(vD));
			if (!Loader.IsBinaryData) Name = s.SerializeString(Name, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
		}
	}
}
