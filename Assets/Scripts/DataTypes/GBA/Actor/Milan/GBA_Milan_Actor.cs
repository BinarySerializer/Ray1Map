namespace R1Engine
{
    public class GBA_Milan_Actor : R1Serializable
    {
        public bool IsCaptor { get; set; } // Set before serializing

        public ushort Index_ActorModel { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public ushort Ushort_06 { get; set; }
        public ushort XlateID { get; set; }

        public ushort LinksCount { get; set; }
        public ActorLink[] Links { get; set; }

        public byte[] UnkBytes { get; set; }

        public ushort DialogCount { get; set; }
        public ushort[] DialogTable { get; set; }

        public byte ActionIndex { get; set; } // TODO: Is this one of the values?

        public override void SerializeImpl(SerializerObject s)
        {
            if (!IsCaptor)
            {
                Index_ActorModel = s.Serialize<ushort>(Index_ActorModel, name: nameof(Index_ActorModel));
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
                XlateID = s.Serialize<ushort>(XlateID, name: nameof(XlateID));

                LinksCount = s.Serialize<ushort>(LinksCount, name: nameof(LinksCount));
                Links = s.SerializeObjectArray<ActorLink>(Links, LinksCount, name: nameof(Links));

                UnkBytes = s.SerializeArray<byte>(UnkBytes, 12, name: nameof(UnkBytes));

                DialogCount = s.Serialize<ushort>(DialogCount, name: nameof(DialogCount));
                DialogTable = s.SerializeArray<ushort>(DialogTable, DialogCount, name: nameof(DialogTable));
            }
            else
            {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
                XlateID = s.Serialize<ushort>(XlateID, name: nameof(XlateID));
                UnkBytes = s.SerializeArray<byte>(UnkBytes, 6, name: nameof(UnkBytes)); // Last ushort here seems to be the level the trigger should load

                Links = new ActorLink[0];
            }
        }

        public class ActorLink : R1Serializable
        {
            public ushort Ushort_00 { get; set; }
            public ushort LinkedActor { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                LinkedActor = s.Serialize<ushort>(LinkedActor, name: nameof(LinkedActor));
            }
        }
    }
}