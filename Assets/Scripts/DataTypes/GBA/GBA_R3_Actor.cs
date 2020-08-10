namespace R1Engine
{
    public class GBA_R3_Actor : R1Serializable
    {
        // Almost always -1
        public int Unk_00 { get; set; }

        public ushort XPos { get; set; }
        public ushort YPos { get; set; }

        public byte Unk_08 { get; set; }
        
        public GBA_R3_ActorID ActorID { get; set; }
        
        public byte Unk_0A { get; set; }

        // Seems to determine the state/animation/palette - this value determines the color of a butterfly for example
        public byte Unk_0B { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk_00 = s.Serialize<int>(Unk_00, name: nameof(Unk_00));

            XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
            YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));

            Unk_08 = s.Serialize<byte>(Unk_08, name: nameof(Unk_08));
            ActorID = s.Serialize<GBA_R3_ActorID>(ActorID, name: nameof(ActorID));
            Unk_0A = s.Serialize<byte>(Unk_0A, name: nameof(Unk_0A));
            Unk_0B = s.Serialize<byte>(Unk_0B, name: nameof(Unk_0B));
        }
    }
}