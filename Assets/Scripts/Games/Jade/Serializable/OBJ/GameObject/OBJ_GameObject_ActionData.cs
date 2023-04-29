using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_ActionData : BinarySerializable {
		public OBJ_GameObject_Base Base { get; set; }
		public Jade_Reference<EVE_ListTracks> ListTracks { get; set; }
		public Jade_Reference<ANI_Shape> Shape { get; set; }
		public Jade_Reference<GRP_Grp> SkeletonGroup { get; set; }

		// Montreal
		public uint BonesVisuCount { get; set; }
		public VisuForBones[] BonesVisu { get; set; }
		public Jade_Reference<ANI_SkeletonInfo> SkeletonInfo { get; set; }
		public byte GOAnimSavedFlags { get; set; }
		public Jade_Reference<AG_AnimGraphFile> AnimGraph { get; set; }

		public Jade_Reference<ACT_ActionKit> ActionKit { get; set; } 

		public override void SerializeImpl(SerializerObject s) {
			// These are resolved later after loading the world
			ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks, name: nameof(ListTracks));
			Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape));
			SkeletonGroup = s.SerializeObject<Jade_Reference<GRP_Grp>>(SkeletonGroup, name: nameof(SkeletonGroup));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP) && Base.GameObject.Version >= 14) {
				SkeletonInfo = s.SerializeObject<Jade_Reference<ANI_SkeletonInfo>>(SkeletonInfo, name: nameof(SkeletonInfo));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Base.GameObject.Version >= 2) {
				BonesVisuCount = s.Serialize<uint>(BonesVisuCount, name: nameof(BonesVisuCount));
				BonesVisu = s.SerializeObjectArray<VisuForBones>(BonesVisu, BonesVisuCount, onPreSerialize: b => b.ActionData = this, name: nameof(BonesVisu));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS) && Base.GameObject.Version >= 19) {
				GOAnimSavedFlags = s.Serialize<byte>(GOAnimSavedFlags, name: nameof(GOAnimSavedFlags));
			}

			// Resolve references here only if not binary data
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (!Loader.IsBinaryData) {
				Shape?.Resolve();
				SkeletonGroup?.Resolve();
				SkeletonInfo?.Resolve();
				ListTracks?.Resolve();
			}

			// This is resolved
			ActionKit = s.SerializeObject<Jade_Reference<ACT_ActionKit>>(ActionKit, name: nameof(ActionKit))?.Resolve();

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP) && Base.GameObject.Version >= 14) {
				AnimGraph = s.SerializeObject<Jade_Reference<AG_AnimGraphFile>>(AnimGraph, name: nameof(AnimGraph));
				if(!ActionKit.IsNull)
					AnimGraph?.Resolve();
			}
		}

		public class VisuForBones : BinarySerializable {
			public OBJ_GameObject_ActionData ActionData { get; set; } // Set in onPreSerialize

			public uint HasVisu { get; set; } // Boolean
			public OBJ_GameObject_Visual Visu { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				HasVisu = s.Serialize<uint>(HasVisu, name: nameof(HasVisu));
				if (HasVisu != 0) {
					Visu = s.SerializeObject<OBJ_GameObject_Visual>(Visu, onPreSerialize: o => o.Version = ActionData.Base.GameObject.Version, name: nameof(Visu));
				}
			}
		}
	}
}
