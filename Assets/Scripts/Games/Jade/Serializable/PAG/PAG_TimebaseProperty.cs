using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in PAG_p_CreateFromBuffer
	public class PAG_TimebaseProperty : BinarySerializable {
		public ushort KeyPointsCount { get; set; }
		public KeyPoint[] KeyPoints { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			KeyPointsCount = s.Serialize<ushort>(KeyPointsCount, name: nameof(KeyPointsCount));
			KeyPoints = s.SerializeObjectArray<KeyPoint>(KeyPoints, KeyPointsCount, name: nameof(KeyPoints));
		}

		public class KeyPoint : BinarySerializable {
			public float Time { get; set; }
			public float Value { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Time = s.Serialize<float>(Time, name: nameof(Time));
				Value = s.Serialize<float>(Value, name: nameof(Value));
			}
		}
	}
}
