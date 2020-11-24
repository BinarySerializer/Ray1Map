namespace R1Engine
{
    public class GBC_Block : R1Serializable
    {
        public byte[] Palm_Header { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
                Palm_Header = s.SerializeArray<byte>(Palm_Header, 4, name: nameof(Palm_Header));
        }
    }
}