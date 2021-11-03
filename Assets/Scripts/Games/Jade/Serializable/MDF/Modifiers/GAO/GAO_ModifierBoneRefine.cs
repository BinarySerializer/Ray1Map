using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierBoneRefine : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }

		public uint V0_UInt_0 { get; set; }
		public uint V0_UInt_1 { get; set; }
		public uint V0_UInt_2 { get; set; }

		public uint Mode { get; set; }
		public float InterpolValue { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			if (Version < 2) {
				V0_UInt_0 = s.Serialize<uint>(V0_UInt_0, name: nameof(V0_UInt_0));
				V0_UInt_1 = s.Serialize<uint>(V0_UInt_1, name: nameof(V0_UInt_1));
				V0_UInt_2 = s.Serialize<uint>(V0_UInt_2, name: nameof(V0_UInt_2));
			}
			if (Version >= 1) Mode = s.Serialize<uint>(Mode, name: nameof(Mode));
			if (Version >= 3) InterpolValue = s.Serialize<float>(InterpolValue, name: nameof(InterpolValue));
		}
	}
}
