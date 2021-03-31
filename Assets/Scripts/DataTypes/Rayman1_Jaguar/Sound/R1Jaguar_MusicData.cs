using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Music data for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_MusicData : BinarySerializable
    {
        public int Time { get; set; } // Time in milliseconds
        public int Command { get; set; }
        /*public byte Channel { get; set; } // 0x81 = play note flags. If off, the note stops. The rest is the channel.
        public ushort Pitch { get; set; } // sample number in upper 4-5 bits?
        public byte Byte7 { get; set; } // either related to duration, sample number or effects. or Volume*/

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Time = s.Serialize<int>(Time, name: nameof(Time));
            Command = s.Serialize<int>(Command, name: nameof(Command));
            /*Channel = s.Serialize<byte>(Channel, name: nameof(Channel));
            Pitch = s.Serialize<ushort>(Pitch, name: nameof(Pitch));
            Byte7 = s.Serialize<byte>(Byte7, name: nameof(Byte7));*/
        }
    }
}