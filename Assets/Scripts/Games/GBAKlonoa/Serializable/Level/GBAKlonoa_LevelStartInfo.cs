using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_LevelStartInfo : BinarySerializable
    {
        public short XPos { get; set; }
        public short YPos { get; set; }
        public bool IsFlipped { get; set; }
        public bool Flag_1 { get; set; }
        public byte Sector { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));

            s.SerializeBitValues<byte>(bitFunc =>
            {
                IsFlipped = bitFunc(IsFlipped ? 1 : 0, 1, name: nameof(IsFlipped)) == 1;
                Flag_1 = bitFunc(Flag_1 ? 1 : 0, 1, name: nameof(Flag_1)) == 1;
                Sector = (byte)bitFunc(Sector, 6, name: nameof(Sector));
            });

            s.SerializePadding(3, logIfNotNull: true);
        }
    }
}