using BinarySerializer;

namespace R1Engine
{
    public class GBAKlonoa_LevelStartInfo : BinarySerializable
    {
        public short XPos { get; set; }
        public short YPos { get; set; }
        public byte Flags { get; set; } // Start direction?

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
            s.SerializePadding(3, logIfNotNull: true);
        }
    }
}