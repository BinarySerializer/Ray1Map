using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierRope : MDF_Modifier {
		public uint Version { get; set; }
		public float V7_Float0 { get; set; }
		public float V8_Float0 { get; set; } = 0.0005f;
		public float V6_Float0 { get; set; }
		public float V4_Float0 { get; set; }
		public float V4_Float1 { get; set; }
		public float V3_Float0 { get; set; }
		public float V3_Float1 { get; set; }
		public float V2_Float0 { get; set; }
		public float V2_Float1 { get; set; }
		public float V1_Float0 { get; set; }
		public Jade_Vector V1_Vector1 { get; set; }
		public float V1_Float2 { get; set; }
		public float V1_Float3 { get; set; }
		public uint V1_UInt4 { get; set; }
		public uint RopeVertexCount { get; set; }
		public RopeVertex[] RopeVertices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 7) V7_Float0 = s.Serialize<float>(V7_Float0, name: nameof(V7_Float0));
			if (Version >= 8) V8_Float0 = s.Serialize<float>(V8_Float0, name: nameof(V8_Float0));
			if (Version >= 6) V6_Float0 = s.Serialize<float>(V6_Float0, name: nameof(V6_Float0));
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
				V1_Float2 = s.Serialize<float>(V1_Float2, name: nameof(V1_Float2));
				V1_Float3 = s.Serialize<float>(V1_Float3, name: nameof(V1_Float3));
				V1_UInt4 = s.Serialize<uint>(V1_UInt4, name: nameof(V1_UInt4));
				RopeVertexCount = s.Serialize<uint>(RopeVertexCount, name: nameof(RopeVertexCount));
				RopeVertices = s.SerializeObjectArray<RopeVertex>(RopeVertices, RopeVertexCount, name: nameof(RopeVertices));
			}
		}

		public class RopeVertex : BinarySerializable {
			public uint UInt0 { get; set; }
			public uint UInt1 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				if (UInt0 != 8) UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
			}
		}
	}
}
