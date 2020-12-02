namespace R1Engine
{
    public class GBC_GameObject : R1Serializable
    {
        public byte Index_ActorModel { get; set; } // Invalid if 0
        public ushort XlateID { get; set; }
        public byte UnkByte0 { get; set; }
        public byte ActorID { get; set; }
        public short YPos { get; set; }
        public short XPos { get; set; }
        public byte UnkByte1 { get; set; }
        public byte ActionID { get; set; }
        public byte LinkCount { get; set; }
        public GBC_GameObjectLink[] Links { get; set; }

        // Actors
        public GBC_UnkActorStruct UnkActorStruct { get; set; }
        public byte UnkByte { get; set; }

        // Captors
        public byte UnkCaptorByte { get; set; }
        public byte BoxHeight { get; set; }
        public byte BoxWidth { get; set; }

        public bool IsCaptor => ActorID == 0xFF;

        // Parsed
        public GBC_ActorModel ActorModel { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index_ActorModel = s.Serialize<byte>(Index_ActorModel, name: nameof(Index_ActorModel));
            XlateID = s.Serialize<ushort>(XlateID, name: nameof(XlateID));
            UnkByte0 = s.Serialize<byte>(UnkByte0, name: nameof(UnkByte0));
            ActorID = s.Serialize<byte>(ActorID, name: nameof(ActorID));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            UnkByte1 = s.Serialize<byte>(UnkByte1, name: nameof(UnkByte1));
            ActionID = s.Serialize<byte>(ActionID, name: nameof(ActionID));
            LinkCount = s.Serialize<byte>(LinkCount, name: nameof(LinkCount));
            Links = s.SerializeObjectArray<GBC_GameObjectLink>(Links, LinkCount, name: nameof(Links));
            if (IsCaptor) {
                UnkCaptorByte = s.Serialize<byte>(UnkCaptorByte, name: nameof(UnkCaptorByte));
                BoxHeight = s.Serialize<byte>(BoxHeight, name: nameof(BoxHeight));
                BoxWidth = s.Serialize<byte>(BoxWidth, name: nameof(BoxWidth));
            } else {
                UnkActorStruct = s.SerializeObject<GBC_UnkActorStruct>(UnkActorStruct, name: nameof(UnkActorStruct));
                UnkByte = s.Serialize<byte>(UnkByte, name: nameof(UnkByte));
            }
        }
    }
}