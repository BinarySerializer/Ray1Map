using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierPhoto : MDF_Modifier {
		public int Mission { get; set; }
		public int Info { get; set; }
		public int BoneForSpherePivot { get; set; }
		public int BoneForInfoPivot { get; set; }
		public float LODMin { get; set; }
		public float LODMax { get; set; }
		public float FrameMin { get; set; }
		public float FrameMax { get; set; }
		public Jade_Vector SphereOffset { get; set; }
		public Jade_Vector InfoOffset { get; set; }
		public float Radius { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Mission = s.Serialize<int>(Mission, name: nameof(Mission));
			Info = s.Serialize<int>(Info, name: nameof(Info));
			BoneForSpherePivot = s.Serialize<int>(BoneForSpherePivot, name: nameof(BoneForSpherePivot));
			BoneForInfoPivot = s.Serialize<int>(BoneForInfoPivot, name: nameof(BoneForInfoPivot));
			LODMin = s.Serialize<float>(LODMin, name: nameof(LODMin));
			LODMax = s.Serialize<float>(LODMax, name: nameof(LODMax));
			FrameMin = s.Serialize<float>(FrameMin, name: nameof(FrameMin));
			FrameMax = s.Serialize<float>(FrameMax, name: nameof(FrameMax));
			if (FrameMax == 1.001f) {
				SphereOffset = s.SerializeObject<Jade_Vector>(SphereOffset, name: nameof(SphereOffset));
				InfoOffset = s.SerializeObject<Jade_Vector>(InfoOffset, name: nameof(InfoOffset));
			}
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
		}
	}
}
