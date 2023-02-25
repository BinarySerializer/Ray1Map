using BinarySerializer;

namespace Ray1Map.Jade {
	public class SPG2_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public float GlobalSize { get; set; }
		public float GlobalRatio { get; set; }
		public float Density { get; set; }
		public uint SubMaterialMask { get; set; }
		public float Noise { get; set; }

		public float Near { get; set; }
		public float Far { get; set; }

		public uint SubMaterial_LOOKAT { get; set; }
		public uint SubMaterial_SPRITE { get; set; }

		public float Extraction { get; set; }
		public uint SubMaterialNum { get; set; }

		public float ExtractionOfHorizontalPlane { get; set; }

		public float SwitchDistance { get; set; }
		public Jade_Key GameObjectKey { get; set; }
		public uint AlphaThreshold { get; set; }
		public uint NumberOfSegments { get; set; }
		public float Gravity { get; set; }
		public float SringStrength { get; set; }
		public float SpeedAbsorbtion { get; set; }
		public float Freedom { get; set; }
		public float WindSensibility { get; set; }
		public uint NumberOfSprites { get; set; }
		public float SpriteSize { get; set; }
		public float SpriteGeneratorRadius { get; set; }
		public uint AlphaFromPondSelector { get; set; }
		public float VShift { get; set; }
		public float Trapeze { get; set; }

		public uint TileNumber { get; set; }

		public uint GridFilter { get; set; }
		
		public float SphereCollideRadius { get; set; }
		public uint Preset { get; set; }

		public float AnimTextureSpeed { get; set; }
		public uint AnimTextureTileUPo2 { get; set; }
		public uint AnimTextureTileVPo2 { get; set; }

		public float GridNoise { get; set; }

		public uint Flags1 { get; set; }

		public uint UniqueID { get; set; }
		public uint BaseNoise { get; set; }

		public float Near2 { get; set; }
		public float Far2 { get; set; }

		public float HeatShimmerNoisePixelSize { get; set; }
		public float HeatShimmerScrollSpeed { get; set; }
		public float HeatShimmerRange { get; set; }
		public float HeatShimmerHeight { get; set; }

		public float HeatShimmerOpacity { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			GlobalSize = s.Serialize<float>(GlobalSize, name: nameof(GlobalSize));
			GlobalRatio = s.Serialize<float>(GlobalRatio, name: nameof(GlobalRatio));
			Density = s.Serialize<float>(Density, name: nameof(Density));
			SubMaterialMask = s.Serialize<uint>(SubMaterialMask, name: nameof(SubMaterialMask));
			Noise = s.Serialize<float>(Noise, name: nameof(Noise));

			if (Version >= 1) {
				Near = s.Serialize<float>(Near, name: nameof(Near));
				Far = s.Serialize<float>(Far, name: nameof(Far));
			}
			if (Version >= 2) {
				SubMaterial_LOOKAT = s.Serialize<uint>(SubMaterial_LOOKAT, name: nameof(SubMaterial_LOOKAT));
				SubMaterial_SPRITE = s.Serialize<uint>(SubMaterial_SPRITE, name: nameof(SubMaterial_SPRITE));
			}
			if (Version >= 3) {
				Extraction = s.Serialize<float>(Extraction, name: nameof(Extraction));
				SubMaterialNum = s.Serialize<uint>(SubMaterialNum, name: nameof(SubMaterialNum));
			}
			if (Version >= 4) {
				ExtractionOfHorizontalPlane = s.Serialize<float>(ExtractionOfHorizontalPlane, name: nameof(ExtractionOfHorizontalPlane));
			}
			if (Version >= 5) {
				SwitchDistance = s.Serialize<float>(SwitchDistance, name: nameof(SwitchDistance));
				GameObjectKey = s.SerializeObject<Jade_Key>(GameObjectKey, name: nameof(GameObjectKey));
				AlphaThreshold = s.Serialize<uint>(AlphaThreshold, name: nameof(AlphaThreshold));
				NumberOfSegments = s.Serialize<uint>(NumberOfSegments, name: nameof(NumberOfSegments));
				Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
				SringStrength = s.Serialize<float>(SringStrength, name: nameof(SringStrength));
				SpeedAbsorbtion = s.Serialize<float>(SpeedAbsorbtion, name: nameof(SpeedAbsorbtion));
				Freedom = s.Serialize<float>(Freedom, name: nameof(Freedom));
				WindSensibility = s.Serialize<float>(WindSensibility, name: nameof(WindSensibility));
				NumberOfSprites = s.Serialize<uint>(NumberOfSprites, name: nameof(NumberOfSprites));
				SpriteSize = s.Serialize<float>(SpriteSize, name: nameof(SpriteSize));
				SpriteGeneratorRadius = s.Serialize<float>(SpriteGeneratorRadius, name: nameof(SpriteGeneratorRadius));
				AlphaFromPondSelector = s.Serialize<uint>(AlphaFromPondSelector, name: nameof(AlphaFromPondSelector));
				VShift = s.Serialize<float>(VShift, name: nameof(VShift));
				Trapeze = s.Serialize<float>(Trapeze, name: nameof(Trapeze));
			}
			if (Version >= 6) TileNumber = s.Serialize<uint>(TileNumber, name: nameof(TileNumber));
			if (Version >= 7) GridFilter = s.Serialize<uint>(GridFilter, name: nameof(GridFilter));
			if (Version >= 8) {
				SphereCollideRadius = s.Serialize<float>(SphereCollideRadius, name: nameof(SphereCollideRadius));
				Preset = s.Serialize<uint>(Preset, name: nameof(Preset));
			}
			if (Version >= 9) {
				AnimTextureSpeed = s.Serialize<float>(AnimTextureSpeed, name: nameof(AnimTextureSpeed));
				AnimTextureTileUPo2 = s.Serialize<uint>(AnimTextureTileUPo2, name: nameof(AnimTextureTileUPo2));
				AnimTextureTileVPo2 = s.Serialize<uint>(AnimTextureTileVPo2, name: nameof(AnimTextureTileVPo2));
			}
			if (Version >= 10) GridNoise = s.Serialize<float>(GridNoise, name: nameof(GridNoise));
			if (Version >= 11) Flags1 = s.Serialize<uint>(Flags1, name: nameof(Flags1));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype)) {
				if (Version >= 12) {
					UniqueID = s.Serialize<uint>(UniqueID, name: nameof(UniqueID));
					BaseNoise = s.Serialize<uint>(BaseNoise, name: nameof(BaseNoise));
				}
				if (Version >= 13) {
					Near2 = s.Serialize<float>(Near2, name: nameof(Near2));
					Far2 = s.Serialize<float>(Far2, name: nameof(Far2));
				}
			}
			if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && Version != 0x111) {
				var maxVersion = (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRRPrototype)) ? 14 : 12;
				if (Version >= maxVersion) {
					HeatShimmerNoisePixelSize = s.Serialize<float>(HeatShimmerNoisePixelSize, name: nameof(HeatShimmerNoisePixelSize));
					HeatShimmerScrollSpeed = s.Serialize<float>(HeatShimmerScrollSpeed, name: nameof(HeatShimmerScrollSpeed));
					HeatShimmerRange = s.Serialize<float>(HeatShimmerRange, name: nameof(HeatShimmerRange));
					HeatShimmerHeight = s.Serialize<float>(HeatShimmerHeight, name: nameof(HeatShimmerHeight));
				}
				if (Version >= maxVersion + 1) {
					HeatShimmerOpacity = s.Serialize<float>(HeatShimmerOpacity, name: nameof(HeatShimmerOpacity));
				}
				if (Version >= 0x111) {
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
				}
			}
		}
	}
}
