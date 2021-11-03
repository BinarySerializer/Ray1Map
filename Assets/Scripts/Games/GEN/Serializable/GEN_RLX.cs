using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map.GEN {
	public class GEN_RLX : BinarySerializable {
		public short RectsCount { get; set; }
		public Rect[] Rects { get; set; }

		public uint? FileSize { get; set; }

		public uint DataLength { get; set; }
		public GEN_RLXData Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if(!FileSize.HasValue) FileSize = s.CurrentLength32;
			if (FileSize == 0) return;

			RectsCount = s.Serialize<short>(RectsCount, name: nameof(RectsCount));
			Rects = s.SerializeObjectArray<Rect>(Rects, RectsCount, name: nameof(Rects));

			DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
			Data = s.SerializeObject<GEN_RLXData>(Data, d => d.FileSize = DataLength, name: nameof(Data));
		}

		public class Rect : BinarySerializable {
			public short X1 { get; set; }
			public short Y1 { get; set; }
			public short X2 { get; set; }
			public short Y2 { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				X1 = s.Serialize<short>(X1, name: nameof(X1));
				Y1 = s.Serialize<short>(Y1, name: nameof(Y1));
				X2 = s.Serialize<short>(X2, name: nameof(X2));
				Y2 = s.Serialize<short>(Y2, name: nameof(Y2));
			}
		}
	}
}