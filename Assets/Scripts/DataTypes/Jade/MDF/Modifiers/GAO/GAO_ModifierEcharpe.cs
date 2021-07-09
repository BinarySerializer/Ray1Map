using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierEcharpe : MDF_Modifier {
		public uint Version { get; set; }
		public float Float1 { get; set; }
		public float Float2 { get; set; }
		public float Float3 { get; set; }
		public float Float4 { get; set; }
		public float Float5 { get; set; }
		public uint UInt6 { get; set; }
		public uint UInt7 { get; set; }
		public uint UInt8 { get; set; }
		public uint UInt9 { get; set; }
		public uint UInt10 { get; set; }
		public Jade_Reference<GEO_Object> GeometricObject { get; set; }
		public float Float12 { get; set; }
		public uint UInt13 { get; set; }
		public float Float14 { get; set; }
		public float Float15 { get; set; }
		public float Float16 { get; set; }
		public Jade_Vector Vector17 { get; set; }
		public uint UInt18 { get; set; }
		public float Float19 { get; set; }
		public uint UInt20 { get; set; }
		public uint VerticesCount { get; set; }
		public EcharpeVertex[] Vertices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Float1 = s.Serialize<float>(Float1, name: nameof(Float1));
			Float2 = s.Serialize<float>(Float2, name: nameof(Float2));
			Float3 = s.Serialize<float>(Float3, name: nameof(Float3));
			Float4 = s.Serialize<float>(Float4, name: nameof(Float4));
			Float5 = s.Serialize<float>(Float5, name: nameof(Float5));
			UInt6 = s.Serialize<uint>(UInt6, name: nameof(UInt6));
			UInt7 = s.Serialize<uint>(UInt7, name: nameof(UInt7));
			UInt8 = s.Serialize<uint>(UInt8, name: nameof(UInt8));
			UInt9 = s.Serialize<uint>(UInt9, name: nameof(UInt9));
			UInt10 = s.Serialize<uint>(UInt10, name: nameof(UInt10));
			GeometricObject = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject, name: nameof(GeometricObject));
			if(Version >= 10) GeometricObject?.Resolve();
			Float12 = s.Serialize<float>(Float12, name: nameof(Float12));
			UInt13 = s.Serialize<uint>(UInt13, name: nameof(UInt13));
			Float14 = s.Serialize<float>(Float14, name: nameof(Float14));
			Float15 = s.Serialize<float>(Float15, name: nameof(Float15));
			Float16 = s.Serialize<float>(Float16, name: nameof(Float16));
			Vector17 = s.SerializeObject<Jade_Vector>(Vector17, name: nameof(Vector17));
			UInt18 = s.Serialize<uint>(UInt18, name: nameof(UInt18));
			Float19 = s.Serialize<float>(Float19, name: nameof(Float19));
			UInt20 = s.Serialize<uint>(UInt20, name: nameof(UInt20));
			VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
			Vertices = s.SerializeObjectArray<EcharpeVertex>(Vertices, VerticesCount, name: nameof(Vertices));
		}

		public class EcharpeVertex : BinarySerializable {
			public uint UInt0 { get; set; }
			public uint UInt1 { get; set; }
			public float Float2 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				UInt1 = s.Serialize<uint>(UInt1, name: nameof(UInt1));
				Float2 = s.Serialize<float>(Float2, name: nameof(Float2));
			}
		}
	}
}
