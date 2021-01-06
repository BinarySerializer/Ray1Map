namespace R1Engine
{
    public class GBA_Milan_Actor : R1Serializable
    {
        public ushort Index_ActorModel { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public ushort ActionIndex { get; set; }
        public ushort XlateID { get; set; }

        public ushort UnkData1Count { get; set; }
        public UnkData_1[] UnkData1 { get; set; }

        public byte[] UnkBytes { get; set; }

        public ushort UnkData2Count { get; set; }
        public ushort[] UnkData2 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Index_ActorModel = s.Serialize<ushort>(Index_ActorModel, name: nameof(Index_ActorModel));
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            ActionIndex = s.Serialize<ushort>(ActionIndex, name: nameof(ActionIndex));
            XlateID = s.Serialize<ushort>(XlateID, name: nameof(XlateID));

            UnkData1Count = s.Serialize<ushort>(UnkData1Count, name: nameof(UnkData1Count));
            UnkData1 = s.SerializeObjectArray<UnkData_1>(UnkData1, UnkData1Count, name: nameof(UnkData1));

            UnkBytes = s.SerializeArray<byte>(UnkBytes, 12, name: nameof(UnkBytes));

            UnkData2Count = s.Serialize<ushort>(UnkData2Count, name: nameof(UnkData2Count));
            UnkData2 = s.SerializeArray<ushort>(UnkData2, UnkData2Count, name: nameof(UnkData2));
        }

        public class UnkData_1 : R1Serializable
        {
            public byte[] Data { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Data = s.SerializeArray<byte>(Data, 4, name: nameof(Data));
            }
        }
    }
}