namespace R1Engine
{
    public class GBC_Actor : R1Serializable
    {
        public byte Index_ActorModel { get; set; } // Invalid if 0
        public ushort Index { get; set; }
        public byte State { get; set; }
        public sbyte ActorID { get; set; }
        public short YPos { get; set; }
        public short XPos { get; set; }
        public byte UnkByte1 { get; set; }
        public byte UnkByte2 { get; set; }
        public byte LinkCount { get; set; }
        public GBC_ActorLink[] Links { get; set; }
        public GBC_UnkActorStruct UnkActorStruct { get; set; }
        public byte UnkByte { get; set; }

        public bool IsCaptor => ActorID == -1;

        // Parsed
        public GBC_ActorModel ActorModel { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index_ActorModel = s.Serialize<byte>(Index_ActorModel, name: nameof(Index_ActorModel));
            Index = s.Serialize<ushort>(Index, name: nameof(Index));
            State = s.Serialize<byte>(State, name: nameof(State));
            ActorID = s.Serialize<sbyte>(ActorID, name: nameof(ActorID));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            UnkByte1 = s.Serialize<byte>(UnkByte1, name: nameof(UnkByte1));
            UnkByte2 = s.Serialize<byte>(UnkByte2, name: nameof(UnkByte2));
            LinkCount = s.Serialize<byte>(LinkCount, name: nameof(LinkCount));
            Links = s.SerializeObjectArray<GBC_ActorLink>(Links, LinkCount, name: nameof(Links));
            UnkActorStruct = s.SerializeObject<GBC_UnkActorStruct>(UnkActorStruct, name: nameof(UnkActorStruct));

            if (!IsCaptor)
                UnkByte = s.Serialize<byte>(UnkByte, name: nameof(UnkByte));
        }
    }
}