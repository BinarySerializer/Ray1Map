using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierRope : MDF_Modifier {
		public uint Version { get; set; }
		public float V4_Float0 { get; set; }
		public float V4_Float1 { get; set; }
		public float V3_Float0 { get; set; }
		public float V3_Float1 { get; set; }
		public float V2_Float0 { get; set; }
		public float V2_Float1 { get; set; }
		public float V1_Float0 { get; set; }
		public Jade_Vector V1_Vector1 { get; set; }
		public uint V1_UInt2 { get; set; }
		public uint V1_UInt3 { get; set; }
		public uint V1_UInt4 { get; set; }
		public uint V1_UInt5_Count { get; set; }
		public V1_Element[] V1_Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 4) {
				V4_Float0 = s.Serialize<float>(V4_Float0, name: nameof(V4_Float0));
				V4_Float1 = s.Serialize<float>(V4_Float1, name: nameof(V4_Float1));
			}
			if (Version >= 3) {
				V3_Float0 = s.Serialize<float>(V3_Float0, name: nameof(V3_Float0));
				V3_Float1 = s.Serialize<float>(V3_Float1, name: nameof(V3_Float1));
			}
			if (Version >= 2) {
				V2_Float0 = s.Serialize<float>(V2_Float0, name: nameof(V2_Float0));
				V2_Float1 = s.Serialize<float>(V2_Float1, name: nameof(V2_Float1));
			}
			if (Version >= 1) {
				V1_Float0 = s.Serialize<float>(V1_Float0, name: nameof(V1_Float0));
				V1_Vector1 = s.SerializeObject<Jade_Vector>(V1_Vector1, name: nameof(V1_Vector1));
				V1_UInt2 = s.Serialize<uint>(V1_UInt2, name: nameof(V1_UInt2));
				V1_UInt3 = s.Serialize<uint>(V1_UInt3, name: nameof(V1_UInt3));
				V1_UInt4 = s.Serialize<uint>(V1_UInt4, name: nameof(V1_UInt4));
				V1_UInt5_Count = s.Serialize<uint>(V1_UInt5_Count, name: nameof(V1_UInt5_Count));
				V1_Elements = s.SerializeObjectArray<V1_Element>(V1_Elements, V1_UInt5_Count, name: nameof(V1_Elements));
			}
		}

		public class V1_Element : BinarySerializable {
			public uint UInt0 { get; set; }
			public uint UInt1 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				if (UInt0 != 8) UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
			}
		}
	}
}
