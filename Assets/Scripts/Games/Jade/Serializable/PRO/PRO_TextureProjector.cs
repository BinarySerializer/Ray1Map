using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PRO_p_TextureProjector_CreateFromBuffer
	public class PRO_TextureProjector : GRO_GraphicRenderObject {
		public float Far { get; set; }
		public float Near { get; set; }
		public float FovX { get; set; }
		public float FovY { get; set; }
		public float FadeInStart { get; set; }
		public float FadeInLength { get; set; }
		public float FadeOutStart { get; set; }
		public float FadeOutLength { get; set; }
		public int IsStatic { get; set; } // Boolean
		public int AffectCharacters { get; set; } // Boolean
		public int Initialized { get; set; } // Unknown, skipped
		public uint ReceiversCount { get; set; }
		public PRO_ReceiverInfo[] Receivers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			FovX = s.Serialize<float>(FovX, name: nameof(FovX));
			FovY = s.Serialize<float>(FovY, name: nameof(FovY));
			FadeInStart = s.Serialize<float>(FadeInStart, name: nameof(FadeInStart));
			FadeInLength = s.Serialize<float>(FadeInLength, name: nameof(FadeInLength));
			FadeOutStart = s.Serialize<float>(FadeOutStart, name: nameof(FadeOutStart));
			FadeOutLength = s.Serialize<float>(FadeOutLength, name: nameof(FadeOutLength));
			IsStatic = s.Serialize<int>(IsStatic, name: nameof(IsStatic));
			AffectCharacters = s.Serialize<int>(AffectCharacters, name: nameof(AffectCharacters));
			Initialized = s.Serialize<int>(Initialized, name: nameof(Initialized));
			ReceiversCount = s.Serialize<uint>(ReceiversCount, name: nameof(ReceiversCount));
			Receivers = s.SerializeObjectArray<PRO_ReceiverInfo>(Receivers, ReceiversCount, onPreSerialize: r => r.Pre_TextureProjector = this, name: nameof(Receivers));
		}
	}
}
