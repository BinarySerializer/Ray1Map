using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GEO_ModiferMeshToParticles : MDF_Modifier {
		public uint Version { get; set; }
		public uint UInt_0 { get; set; }
		public uint Flags { get; set; }
		public byte Byte_4 { get; set; }
		public byte Byte_2 { get; set; }
		public uint ShortCount { get; set; }
		public short[] Shorts { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_0 = s.Serialize<uint>(UInt_0, name: nameof(UInt_0));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if (Version >= 1) {
				if ((Flags & 4) != 0) Byte_4 = s.Serialize<byte>(Byte_4, name: nameof(Byte_4));
				if ((Flags & 2) != 0) Byte_2 = s.Serialize<byte>(Byte_2, name: nameof(Byte_2));
				if ((Flags & 6) != 0 && Version == 1) {
					ShortCount = s.Serialize<uint>(ShortCount, name: nameof(ShortCount));
					Shorts = s.SerializeArray<short>(Shorts, ShortCount, name: nameof(Shorts));
				}
			}
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
		}
	}
}
