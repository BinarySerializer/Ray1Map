using BinarySerializer;

namespace Ray1Map.Jade {
	public class MDF_AudioAmbiancePortal : MDF_Modifier {
		public uint DataSize { get; set; }
		public uint Version { get; set; }

		public uint Flags { get; set; }
		public uint TriggerMode { get; set; }
		public uint SoundEvent { get; set; }

		public uint ReverbEventRed { get; set; }
		public uint ReverbEventBlue { get; set; }
		public Rectangle[] Rectangles { get; set; }
		public float[] Range { get; set; }
		public OBBox[] PrefetchBox { get; set; }
		public float RedOutputAtRectangle { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			TriggerMode = s.Serialize<uint>(TriggerMode, name: nameof(TriggerMode));
			SoundEvent = s.Serialize<uint>(SoundEvent, name: nameof(SoundEvent));
			if (Version >= 2) {
				ReverbEventRed = s.Serialize<uint>(ReverbEventRed, name: nameof(ReverbEventRed));
				ReverbEventBlue = s.Serialize<uint>(ReverbEventBlue, name: nameof(ReverbEventBlue));
			}
			Rectangles = s.SerializeObjectArray<Rectangle>(Rectangles, 2, name: nameof(Rectangles));
			Range = s.SerializeArray<float>(Range, 2, name: nameof(Range));
			PrefetchBox = s.SerializeObjectArray<OBBox>(PrefetchBox, 2, name: nameof(PrefetchBox));
			RedOutputAtRectangle = s.Serialize<float>(RedOutputAtRectangle, name: nameof(RedOutputAtRectangle));
		}

		public class Rectangle : BinarySerializable {
			public float ExtentsX { get; set; }
			public float ExtentsY { get; set; }
			public float OffsetX { get; set; }
			public float OffsetY { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				ExtentsX = s.Serialize<float>(ExtentsX, name: nameof(ExtentsX));
				ExtentsY = s.Serialize<float>(ExtentsY, name: nameof(ExtentsY));
				OffsetX = s.Serialize<float>(OffsetX, name: nameof(OffsetX));
				OffsetY = s.Serialize<float>(OffsetY, name: nameof(OffsetY));
			}
		}

		public class OBBox : BinarySerializable {
			public Jade_Vector Center { get; set; }
			public Jade_Vector[] Axis { get; set; }
			public Jade_Vector Extent { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Center = s.SerializeObject<Jade_Vector>(Center, name: nameof(Center));
				Axis = s.SerializeObjectArray<Jade_Vector>(Axis, 3, name: nameof(Axis));
				Extent = s.SerializeObject<Jade_Vector>(Extent, name: nameof(Extent));
			}
		}
	}
}
