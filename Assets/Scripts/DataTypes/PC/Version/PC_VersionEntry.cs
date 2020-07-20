namespace R1Engine
{
    public class PC_VersionEntry : R1Serializable
    {
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public uint Unk5 { get; set; }
        public uint Unk6 { get; set; }
        public uint Unk7 { get; set; }
        public uint Unk8 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<uint>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));
            Unk5 = s.Serialize<uint>(Unk5, name: nameof(Unk5));
            Unk6 = s.Serialize<uint>(Unk6, name: nameof(Unk6));

            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC)
            {
                Unk7 = s.Serialize<uint>(Unk7, name: nameof(Unk7));
                Unk8 = s.Serialize<uint>(Unk8, name: nameof(Unk8));
            }
        }
    }
}