using System.Text;
using BinarySerializer;

namespace Ray1Map {
    public class MusyX_SongTable : BinarySerializable {
        // Set in OnPreSerialize
        public Pointer BaseOffset { get; set; }
        public Pointer EndOffset { get; set; }

        public uint Length { get; set; }
        public Pointer[] Songs { get; set; }

        public byte[][] SongBytes { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Songs = s.SerializePointerArray(Songs, Length, anchor: BaseOffset, name: nameof(Songs));

            if (SongBytes == null) {
                SongBytes = new byte[Songs.Length][];
                for (int i = 0; i < Songs.Length; i++) {
                    Pointer nextOff = (i < Songs.Length - 1) ? Songs[i + 1] : EndOffset;
                    s.DoAt(Songs[i], () => {
                        SongBytes[i] = s.SerializeArray<byte>(SongBytes[i], nextOff - Songs[i], name: $"{nameof(SongBytes)}[{i}]");
                    });
                }
            }
        }
    }
}