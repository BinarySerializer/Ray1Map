using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_ActionData : BinarySerializable {
		public OBJ_GameObject_Base Base { get; set; }
		public Jade_Reference<EVE_ListTracks> ListTracks { get; set; }
		public Jade_Reference<ANI_Shape> Shape { get; set; }
		public Jade_Reference<GRP_Grp> SkeletonGroup { get; set; }

		// Montreal
		public uint BonesVisuCount { get; set; }
		public VisuForBones[] BonesVisu { get; set; }

		public Jade_Reference<ACT_ActionKit> ActionKit { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// These are resolved later after loading the world
			ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks, name: nameof(ListTracks));
			Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape));
			SkeletonGroup = s.SerializeObject<Jade_Reference<GRP_Grp>>(SkeletonGroup, name: nameof(SkeletonGroup));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Base.GameObject.Version >= 2) {
				BonesVisuCount = s.Serialize<uint>(BonesVisuCount, name: nameof(BonesVisuCount));
				BonesVisu = s.SerializeObjectArray<VisuForBones>(BonesVisu, BonesVisuCount, onPreSerialize: b => b.ActionData = this, name: nameof(BonesVisu));
			}

			// This is resolved
			ActionKit = s.SerializeObject<Jade_Reference<ACT_ActionKit>>(ActionKit, name: nameof(ActionKit))?.Resolve();
		}

		public class VisuForBones : BinarySerializable {
			public OBJ_GameObject_ActionData ActionData { get; set; } // Set in onPreSerialize

			public uint Unknown { get; set; }
			public OBJ_GameObject_Visual Visu { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
				Visu = s.SerializeObject<OBJ_GameObject_Visual>(Visu, onPreSerialize: o => o.Version = ActionData.Base.GameObject.Version, name: nameof(Visu));
			}
		}
	}
}
