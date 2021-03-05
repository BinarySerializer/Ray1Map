namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_POP : R1Serializable
    {
        public string Magic { get; set; }
        public GBAVV_NitroKart_NGage_BackgroundLayer[] BackgroundLayers { get; set; }
        public GBAVV_NitroKart_NGage_ObjectCollection[] ObjectCollections { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            BackgroundLayers = s.SerializeObjectArray<GBAVV_NitroKart_NGage_BackgroundLayer>(BackgroundLayers, 3, name: nameof(BackgroundLayers));
            ObjectCollections = s.SerializeObjectArray<GBAVV_NitroKart_NGage_ObjectCollection>(ObjectCollections, 6, name: nameof(ObjectCollections));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}