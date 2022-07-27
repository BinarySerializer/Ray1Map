using BinarySerializer;

namespace Ray1Map.Jade {
	public class DARE_ModifierSound : MDF_Modifier {
		public uint DataSize { get; set; }
		public uint Version { get; set; }

		public uint Flag { get; set; }
		public uint PlayEventsCount { get; set; }
		public Event[] PlayEvents { get; set; }
		public uint StopEventsCount { get; set; }
		public Event[] StopEvents { get; set; }
		public Zone PlayZone { get; set; } // Later called Entry Zone
		public Zone StopZone { get; set; } // Later called Exit Zone
		public Trigger TriggerMode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
			Version = s.Serialize<uint>(Version, name: nameof(Version));

			switch (Version) {
				case 3:
				case 4:
				case 5:
					SerializeImpl_Version3(s);
					break;
				default:
					s.SystemLog?.LogWarning($"{Offset}: DARE_ModifierSound: Unhandled version {Version}");
					break;
			}

			uint readSize = (uint)(s.CurrentPointer - Offset);
			if (readSize != DataSize) {
				s.SystemLog?.LogWarning($"{Offset}: DARE_ModifierSound was not fully serialized: Data Size: {DataSize:X8} / Serialized: {readSize:X8}");
				s.Goto(Offset + DataSize);
			}
		}
		public void SerializeImpl_Version3(SerializerObject s) {
			Flag = s.Serialize<uint>(Flag, name: nameof(Flag));
			PlayEventsCount = s.Serialize<uint>(PlayEventsCount, name: nameof(PlayEventsCount));
			PlayEvents = s.SerializeObjectArray<Event>(PlayEvents, PlayEventsCount, onPreSerialize: e => e.Version = Version, name: nameof(PlayEvents));
			StopEventsCount = s.Serialize<uint>(StopEventsCount, name: nameof(StopEventsCount));
			StopEvents = s.SerializeObjectArray<Event>(StopEvents, StopEventsCount, onPreSerialize: e => e.Version = Version, name: nameof(StopEvents));

			PlayZone = s.SerializeObject<Zone>(PlayZone, name: nameof(PlayZone));
			if (StopEventsCount == 0 || (Flag & 0x10) != 0) {
				StopZone = PlayZone;
			} else {
				StopZone = s.SerializeObject<Zone>(StopZone, name: nameof(StopZone));
			}
			if (Version >= 5) {
				TriggerMode = s.Serialize<Trigger>(TriggerMode, name: nameof(TriggerMode));
			}
		}

		public class Event : BinarySerializable {
			public uint Version { get; set; }

			public uint EventValue { get; set; }
			public uint MusicMessage { get; set; }
			public uint Type { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				EventValue = s.Serialize<uint>(EventValue, name: nameof(EventValue));
				MusicMessage = s.Serialize<uint>(MusicMessage, name: nameof(MusicMessage));
				if (Version >= 5) {
					Type = s.Serialize<uint>(Type, name: nameof(Type));
				}
			}
		}
		public class Zone : BinarySerializable {
			public COL_ZoneShape Type { get; set; }
			public COL_Box Shape_Box { get; set; } // Type 1
			public COL_Sphere Shape_Sphere { get; set; } // Type 2

			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<COL_ZoneShape>(Type, name: nameof(Type));
				switch (Type) {
					case COL_ZoneShape.Box:
						Shape_Box = s.SerializeObject<COL_Box>(Shape_Box, name: nameof(Shape_Box));
						break;

					case COL_ZoneShape.Sphere:
						Shape_Sphere = s.SerializeObject<COL_Sphere>(Shape_Sphere, name: nameof(Shape_Sphere));
						break;
				}
			}
		}

		public enum Trigger : uint {
			Actor = 0,
			Microphone = 1,
			Both = 2,
			Count = 3,
		}
	}
}
