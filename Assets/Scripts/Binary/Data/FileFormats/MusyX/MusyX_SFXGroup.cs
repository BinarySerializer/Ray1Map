using System.Text;
using BinarySerializer;

namespace Ray1Map {
    public class MusyX_SFXGroup : BinarySerializable {
        // Set in OnPreSerialize
        public Pointer BaseOffset { get; set; }

        public uint Length { get; set; }
        public SoundEffect[] Entries { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Entries = s.SerializeObjectArray<SoundEffect>(Entries, Length, name: nameof(Entries));
        }

        public class SoundEffect : BinarySerializable {
            public ushort InstrumentIndex { get; set; }
            public byte ObjectID { get; set; }
            public byte Priority { get; set; }
            public byte MaxVoicesCount { get; set; }
            public byte DefaultVelocity { get; set; } // Volume - usually 0x7F
            public byte DefaultPanning { get; set; }
            public byte DefaultKey { get; set; } // The default pitch - usually 0x3C (MIDI C4)

			public override void SerializeImpl(SerializerObject s) {
				InstrumentIndex = s.Serialize<ushort>(InstrumentIndex, name: nameof(InstrumentIndex));
				ObjectID = s.Serialize<byte>(ObjectID, name: nameof(ObjectID));
				Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
				MaxVoicesCount = s.Serialize<byte>(MaxVoicesCount, name: nameof(MaxVoicesCount));
				DefaultVelocity = s.Serialize<byte>(DefaultVelocity, name: nameof(DefaultVelocity));
				DefaultPanning = s.Serialize<byte>(DefaultPanning, name: nameof(DefaultPanning));
				DefaultKey = s.Serialize<byte>(DefaultKey, name: nameof(DefaultKey));
			}
		}
    }
}