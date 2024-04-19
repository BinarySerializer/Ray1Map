using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierMotionBlur : MDF_Modifier {
		public uint Version { get; set; }
		public uint Size { get; set; }
		public float BlurLengthFactor { get; set; }
		public float BlurAlphaFactor { get; set; }
		public int UseForcedColor { get; set; }
		public SerializableColor ForcedColor { get; set; }
		public uint NumberMatrixUsed { get; set; }
		public MotionBlurType Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Size = s.Serialize<uint>(Size, name: nameof(Size));
			BlurLengthFactor = s.Serialize<float>(BlurLengthFactor, name: nameof(BlurLengthFactor));
			BlurAlphaFactor = s.Serialize<float>(BlurAlphaFactor, name: nameof(BlurAlphaFactor));
			UseForcedColor = s.Serialize<int>(UseForcedColor, name: nameof(UseForcedColor));
			ForcedColor = s.SerializeInto<SerializableColor>(ForcedColor, BitwiseColor.RGBA8888, name: nameof(ForcedColor));
			if (Version >= 2) {
				NumberMatrixUsed = s.Serialize<uint>(NumberMatrixUsed, name: nameof(NumberMatrixUsed));
				Type = s.Serialize<MotionBlurType>(Type, name: nameof(Type));
			}
		}

		public enum MotionBlurType : uint {
			WholeBlur_1to0 = 0,
			WholeBlur_From0to1to0 = 1,
			ByPass_1to0 = 2,
			WholeBlur_1to0_ByPass_1to0 = 3,
			Last = 4,
		}
	}
}
