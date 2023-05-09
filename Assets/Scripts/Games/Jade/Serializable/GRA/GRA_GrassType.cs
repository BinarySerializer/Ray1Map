using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_GrassType : BinarySerializable {
		public uint Pre_ObjectVersion { get; set; }

		public Jade_Reference<OBJ_GameObject> GrassMesh { get; set; }
		public uint Flags { get; set; }
		public float ScaleWidth { get; set; }
		public float ScaleHeight { get; set; }
		public float SizeRandomnessFactor { get; set; }
		public float Probability { get; set; }
		public float WindResponse { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GrassMesh = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GrassMesh, name: nameof(GrassMesh))?.Resolve();
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			ScaleWidth = s.Serialize<float>(ScaleWidth, name: nameof(ScaleWidth));
			if (Pre_ObjectVersion >= 2)
				ScaleHeight = s.Serialize<float>(ScaleHeight, name: nameof(ScaleHeight));
			else
				ScaleHeight = ScaleWidth;
			if (Pre_ObjectVersion >= 3) SizeRandomnessFactor = s.Serialize<float>(SizeRandomnessFactor, name: nameof(SizeRandomnessFactor));
			Probability = s.Serialize<float>(Probability, name: nameof(Probability));
			if (Pre_ObjectVersion >= 4) WindResponse = s.Serialize<float>(WindResponse, name: nameof(WindResponse));
		}
	}
}
