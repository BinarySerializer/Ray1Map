using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierFOGDY : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public float XSize { get; set; }
		public float YSize { get; set; }
		public float ZSize { get; set; }

		public SerializableColor Colors1 { get; set; }
		public SerializableColor Colors2 { get; set; }
		public SerializableColor Colors3 { get; set; }
		public float TimePhase1 { get; set; }
		public float TimePhase2 { get; set; }
		public float SpeedMin { get; set; }
		public float SpeedMax { get; set; }
		public float SizeMin { get; set; }
		public float SizeMax { get; set; }
		public float GrowingMin { get; set; }
		public float GrowingMax { get; set; }
		public float FrictionGrow { get; set; }
		public Jade_Vector FrictionSpeed { get; set; }
		public float Gravity { get; set; }

		public float TimeVariance { get; set; }
		public uint NumberOfActiveSprites { get; set; }
		public float GenerationRate { get; set; }

		public uint CollisionMode { get; set; }
		public float PushPower { get; set; }
		public float ExtractionSpeed { get; set; }
		public float FrictionLength { get; set; }

		public uint ActiveChannel { get; set; }
		public uint SubMaterialNum1 { get; set; }

		public uint SubMaterialNum2 { get; set; }
		public uint SubMaterialNum3 { get; set; }
		public uint SubMaterialNum4 { get; set; }

		public float SpeedStart { get; set; }
		public float MaxSpeed { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version < 3) {
				XSize = s.Serialize<float>(XSize, name: nameof(XSize));
				YSize = s.Serialize<float>(YSize, name: nameof(YSize));
				ZSize = s.Serialize<float>(ZSize, name: nameof(ZSize));
			}
			if (Version >= 1) {
				Colors1 = s.SerializeInto<SerializableColor>(Colors1, BitwiseColor.RGBA8888, name: nameof(Colors1));
				Colors2 = s.SerializeInto<SerializableColor>(Colors2, BitwiseColor.RGBA8888, name: nameof(Colors2));
				Colors3 = s.SerializeInto<SerializableColor>(Colors3, BitwiseColor.RGBA8888, name: nameof(Colors3));
				TimePhase1 = s.Serialize<float>(TimePhase1, name: nameof(TimePhase1));
				TimePhase2 = s.Serialize<float>(TimePhase2, name: nameof(TimePhase2));
				SpeedMin = s.Serialize<float>(SpeedMin, name: nameof(SpeedMin));
				SpeedMax = s.Serialize<float>(SpeedMax, name: nameof(SpeedMax));
				SizeMin = s.Serialize<float>(SizeMin, name: nameof(SizeMin));
				SizeMax = s.Serialize<float>(SizeMax, name: nameof(SizeMax));
				GrowingMin = s.Serialize<float>(GrowingMin, name: nameof(GrowingMin));
				GrowingMax = s.Serialize<float>(GrowingMax, name: nameof(GrowingMax));
				FrictionGrow = s.Serialize<float>(FrictionGrow, name: nameof(FrictionGrow));
				FrictionSpeed = s.SerializeObject<Jade_Vector>(FrictionSpeed, name: nameof(FrictionSpeed));
				Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
			}
			if (Version >= 2) {
				TimeVariance = s.Serialize<float>(TimeVariance, name: nameof(TimeVariance));
				NumberOfActiveSprites = s.Serialize<uint>(NumberOfActiveSprites, name: nameof(NumberOfActiveSprites));
				GenerationRate = s.Serialize<float>(GenerationRate, name: nameof(GenerationRate));
			}
			if (Version >= 4) ZSize = s.Serialize<float>(ZSize, name: nameof(ZSize));
			if (Version >= 5) CollisionMode = s.Serialize<uint>(CollisionMode, name: nameof(CollisionMode));
			if (Version >= 6) PushPower = s.Serialize<float>(PushPower, name: nameof(PushPower));
			if (Version >= 7) ExtractionSpeed = s.Serialize<float>(ExtractionSpeed, name: nameof(ExtractionSpeed));
			if (Version >= 8) FrictionLength = s.Serialize<float>(FrictionLength, name: nameof(FrictionLength));
			if (Version >= 9) {
				ActiveChannel = s.Serialize<uint>(ActiveChannel, name: nameof(ActiveChannel));
				SubMaterialNum1 = s.Serialize<uint>(SubMaterialNum1, name: nameof(SubMaterialNum1));
			}
			if (Version >= 10) {
				SubMaterialNum2 = s.Serialize<uint>(SubMaterialNum2, name: nameof(SubMaterialNum2));
				SubMaterialNum3 = s.Serialize<uint>(SubMaterialNum3, name: nameof(SubMaterialNum3));
				SubMaterialNum4 = s.Serialize<uint>(SubMaterialNum4, name: nameof(SubMaterialNum4));
			}
			if (Version >= 11) {
				SpeedStart = s.Serialize<float>(SpeedStart, name: nameof(SpeedStart));
				MaxSpeed = s.Serialize<float>(MaxSpeed, name: nameof(MaxSpeed));
			}
		}
	}
}
