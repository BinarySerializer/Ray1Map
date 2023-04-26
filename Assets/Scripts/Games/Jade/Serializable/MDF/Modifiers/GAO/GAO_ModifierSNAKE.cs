using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierSNAKE : MDF_Modifier {
		public uint Version { get; set; }
		public uint BonesCount { get; set; }
		public float Inertie { get; set; }
		public float BlendDist { get; set; }
		public float Attenuation { get; set; }
		public int[] Bones { get; set; }
		public uint Flags { get; set; }

		// CPP
		public uint CPP_VersionOrBonesCount { get; set; }
		public uint CPP_Version { get; set; } = 1;
		public bool CPP_SerializeVersion { get; set; }
		public float Gravity { get; set; }
		public uint CollisionPlanesCount { get; set; }
		public CollisionPlane[] CollisionPlanes { get; set; }
		public float Untwist { get; set; }
		public byte MainAxis { get; set; }
		public byte UpAxis { get; set; }
		public float FadeTimeCfg { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong) || !Loader.IsBinaryData) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
				s.DoBits<uint>(b => {
					CPP_VersionOrBonesCount = b.SerializeBits<uint>(CPP_VersionOrBonesCount, 31, name: nameof(CPP_VersionOrBonesCount));
					CPP_SerializeVersion = b.SerializeBits<bool>(CPP_SerializeVersion, 1, name: nameof(CPP_SerializeVersion));
				});
				if (CPP_SerializeVersion) {
					BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
					CPP_Version = CPP_VersionOrBonesCount;
				} else {
					BonesCount = CPP_VersionOrBonesCount;
					CPP_Version = 1;
				}
			} else {
				BonesCount = s.Serialize<uint>(BonesCount, name: nameof(BonesCount));
			}
			Inertie = s.Serialize<float>(Inertie, name: nameof(Inertie));
			BlendDist = s.Serialize<float>(BlendDist, name: nameof(BlendDist));
			Attenuation = s.Serialize<float>(Attenuation, name: nameof(Attenuation));
			Bones = s.SerializeArray<int>(Bones, CPP_Version >= 4 ? BonesCount : 20, name: nameof(Bones));

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				if (Version >= 9) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			} else {
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002)) {
					Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
					if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_CPP)) {
						if (CPP_Version >= 2) Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
						if (CPP_Version >= 7) {
							CollisionPlanesCount = s.Serialize<uint>(CollisionPlanesCount, name: nameof(CollisionPlanesCount));
						} else {
							CollisionPlanesCount = 1;
						}
						CollisionPlanes = s.SerializeObjectArray<CollisionPlane>(CollisionPlanes, CollisionPlanesCount, onPreSerialize: c => c.Pre_Version = CPP_Version, name: nameof(CollisionPlanes));
						if (CPP_Version >= 5) {
							Untwist = s.Serialize<float>(Untwist, name: nameof(Untwist));
							MainAxis = s.Serialize<byte>(MainAxis, name: nameof(MainAxis));
							UpAxis = s.Serialize<byte>(UpAxis, name: nameof(UpAxis));
						}
						if (CPP_Version >= 6) FadeTimeCfg = s.Serialize<float>(FadeTimeCfg, name: nameof(FadeTimeCfg));
					}
				}
			}
		}

		public class CollisionPlane : BinarySerializable {
			public uint Pre_Version { get; set; }

			public byte Bone { get; set; }
			public byte Bone1 { get; set; }
			public byte Bone2 { get; set; }
			public Jade_Vector Direction { get; set; }
			public float Distance { get; set; }
			public Jade_Vector BoxMin { get; set; } = new Jade_Vector(-1000, -1000, -1000);
			public Jade_Vector BoxMax { get; set; } = new Jade_Vector( 1000,  1000,  1000);
			public Jade_Vector[] BoneTriangleOffsets { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Bone = s.Serialize<byte>(Bone, name: nameof(Bone));
				Direction = s.SerializeObject<Jade_Vector>(Direction, name: nameof(Direction));
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
				if (Pre_Version >= 3) {
					BoxMin = s.SerializeObject<Jade_Vector>(BoxMin, name: nameof(BoxMin));
					BoxMin = s.SerializeObject<Jade_Vector>(BoxMin, name: nameof(BoxMin));
				}
				if (Pre_Version >= 8) {
					Bone1 = s.Serialize<byte>(Bone1, name: nameof(Bone1));
					Bone2 = s.Serialize<byte>(Bone2, name: nameof(Bone2));
					BoneTriangleOffsets = s.SerializeObjectArray<Jade_Vector>(BoneTriangleOffsets, 3, name: nameof(BoneTriangleOffsets));
				}
			}
		}
	}
}
