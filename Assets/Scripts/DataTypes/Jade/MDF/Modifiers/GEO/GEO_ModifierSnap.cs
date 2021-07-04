using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GEO_ModifierSnap : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public uint PointsCount { get; set; }
		public Point[] Points { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
			Points = s.SerializeObjectArray<Point>(Points, PointsCount, name: nameof(Points));
		}

		public class Point : BinarySerializable {
			public uint IndexSource { get; set; }
			public uint IndexTarget { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				IndexSource = s.Serialize<uint>(IndexSource, name: nameof(IndexSource));
				IndexTarget = s.Serialize<uint>(IndexTarget, name: nameof(IndexTarget));
			}
		}
	}
}
