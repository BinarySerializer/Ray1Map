namespace R1Engine
{
    public class GBARRR_Scene : R1Serializable
    {
        public bool IsUnusedMode7 { get; set; }

        public ushort Unk1 { get; set; } // Always 0?
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public uint ActorCount { get; set; }
        public byte[] UnkData { get; set; }

        public GBARRR_Actor[] Actors { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));
            Unk3 = s.Serialize<ushort>(Unk3, name: nameof(Unk3));
            ActorCount = s.Serialize<uint>(ActorCount, name: nameof(ActorCount));
            UnkData = s.SerializeArray<byte>(UnkData, IsUnusedMode7 ? 166 : 230, name: nameof(UnkData));

            Actors = s.SerializeObjectArray<GBARRR_Actor>(Actors, ActorCount, name: nameof(Actors));
            s.Serialize<uint>(0, "Padding");
        }
    }
}