using BinarySerializer;

namespace Ray1Map.Jade
{
	public class AG_TriggerBind : BinarySerializable {
		public uint Pre_Version { get; set; }

		public byte Hold { get; set; }
		public byte BeginSequence { get; set; }
		public AG_Bitfield JustPressed { get; set; }
		public AG_Bitfield Pressed { get; set; }
		public AG_Bitfield JustReleased { get; set; }
		public AG_Bitfield Released { get; set; }
		public AG_Bitfield JustPressedWithDelay { get; set; }
		public int AnalogCount { get; set; }
		public GearMapEntry[] Analog { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Hold = s.Serialize<byte>(Hold, name: nameof(Hold));
			if(Pre_Version >= 1) BeginSequence = s.Serialize<byte>(BeginSequence, name: nameof(BeginSequence));
			JustPressed = s.SerializeObject<AG_Bitfield>(JustPressed, name: nameof(JustPressed));
			Pressed = s.SerializeObject<AG_Bitfield>(Pressed, name: nameof(Pressed));
			JustReleased = s.SerializeObject<AG_Bitfield>(JustReleased, name: nameof(JustReleased));
			Released = s.SerializeObject<AG_Bitfield>(Released, name: nameof(Released));
			JustPressedWithDelay = s.SerializeObject<AG_Bitfield>(JustPressedWithDelay, name: nameof(JustPressedWithDelay));
			AnalogCount = s.Serialize<int>(AnalogCount, name: nameof(AnalogCount));
			Analog = s.SerializeObjectArray<GearMapEntry>(Analog, AnalogCount, name: nameof(Analog));
		}

		public class GearMapEntry : BinarySerializable {
			public int Int_00 { get; set; }
			public int Key { get; set; }
			public float Value { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
				Key = s.Serialize<int>(Key, name: nameof(Key));
				Value = s.Serialize<float>(Value, name: nameof(Value));
			}
		}

	}
}