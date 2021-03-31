using System.Text;
using BinarySerializer;

namespace R1Engine {
    public class MusyX_UnknownTable : BinarySerializable {
        // Set in OnPreSerialize
        public Pointer BaseOffset { get; set; }

        public uint Length { get; set; }
        public Entry[] Entries { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Entries = s.SerializeObjectArray<Entry>(Entries, Length, name: nameof(Entries));
        }

        public class Entry : BinarySerializable {
            public byte[] Bytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Bytes = s.SerializeArray<byte>(Bytes, 8, name: nameof(Bytes));
			}
		}
    }
}