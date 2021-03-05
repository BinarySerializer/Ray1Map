namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_POP : R1Serializable
    {
        public string Magic { get; set; }
        public GBAVV_NitroKart_NGage_BackgroundLayer[] BackgroundLayers { get; set; }
        public GBAVV_NitroKart_NGage_ObjectCollection Objects_Normal { get; set; }
        public GBAVV_NitroKart_NGage_ObjectCollection Objects_TimeTrial { get; set; }
        public GBAVV_NitroKart_NGage_ObjectCollection Objects_BossRace { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            BackgroundLayers = s.SerializeObjectArray<GBAVV_NitroKart_NGage_BackgroundLayer>(BackgroundLayers, 6, name: nameof(BackgroundLayers));
            Objects_Normal = s.SerializeObject<GBAVV_NitroKart_NGage_ObjectCollection>(Objects_Normal, name: nameof(Objects_Normal));
            Objects_TimeTrial = s.SerializeObject<GBAVV_NitroKart_NGage_ObjectCollection>(Objects_TimeTrial, name: nameof(Objects_TimeTrial));
            Objects_BossRace = s.SerializeObject<GBAVV_NitroKart_NGage_ObjectCollection>(Objects_BossRace, name: nameof(Objects_BossRace));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}