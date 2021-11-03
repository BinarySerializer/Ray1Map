using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_POP : BinarySerializable
    {
        public string Magic { get; set; }
        public GBAVV_NitroKart_TrackData TrackData1 { get; set; }
        public GBAVV_NitroKart_TrackData TrackData2 { get; set; }
        public GBAVV_NitroKart_Objects Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            TrackData1 = s.SerializeObject<GBAVV_NitroKart_TrackData>(TrackData1, name: nameof(TrackData1));
            TrackData2 = s.SerializeObject<GBAVV_NitroKart_TrackData>(TrackData2, name: nameof(TrackData2));
            Objects = s.SerializeObject<GBAVV_NitroKart_Objects>(Objects, name: nameof(Objects));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}