using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierXMEN : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Type { get; set; }
		public uint Flags { get; set; }
		public uint PointsCount { get; set; }
		public Point[] Points { get; set; }
		public Jade_Reference<GEO_Object> GeometricObject { get; set; }
		public uint Type1_UInt { get; set; }
		public float Type2_Float { get; set; }
		public uint Type3_UInt { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
			Points = s.SerializeObjectArray<Point>(Points, PointsCount, name: nameof(Points));
			GeometricObject = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject, name: nameof(GeometricObject));
			if(Type >= 1) Type1_UInt = s.Serialize<uint>(Type1_UInt, name: nameof(Type1_UInt));
			if(Type >= 2) Type2_Float = s.Serialize<float>(Type2_Float, name: nameof(Type2_Float));
			if(Type >= 3) Type3_UInt = s.Serialize<uint>(Type3_UInt, name: nameof(Type3_UInt));
			//if ((Flags & 8) == 0) { // This is always 0 -- it does Flags &= 0xFFFFFFF7 after reading it
				GeometricObject?.Resolve();
			//}
		}

		public class Point : BinarySerializable {
			public uint Index { get; set; }
			public float Float { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Index = s.Serialize<uint>(Index, name: nameof(Index));
				Float = s.Serialize<float>(Float, name: nameof(Float));
			}
		}
	}
}
