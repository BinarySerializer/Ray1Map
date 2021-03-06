namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_POP : R1Serializable
    {
        public string Magic { get; set; }
        public GBAVV_NitroKart_TrackData TrackData { get; set; }
        public GBAVV_NitroKart_NGage_UnknownStruct[] UnknownStruct { get; set; }
        public GBAVV_NitroKart_Objects Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            TrackData = s.SerializeObject<GBAVV_NitroKart_TrackData>(TrackData, name: nameof(TrackData));
            UnknownStruct = s.SerializeObjectArray<GBAVV_NitroKart_NGage_UnknownStruct>(UnknownStruct, 3, name: nameof(UnknownStruct));
            Objects = s.SerializeObject<GBAVV_NitroKart_Objects>(Objects, name: nameof(Objects));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}