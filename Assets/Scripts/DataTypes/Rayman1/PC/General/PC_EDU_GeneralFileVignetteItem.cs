namespace R1Engine
{
    public class PC_EDU_GeneralFileVignetteItem : R1Serializable
    {
        public ushort Unk1 { get; set; }
        public byte Unk2 { get; set; }

        public string VignetteName { get; set; }

        public uint Unk3 { get; set; }

        public uint Length { get; set; }

        // Pointer?
        public uint Unk4 { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<byte>(Unk2, name: nameof(Unk2));
            VignetteName = s.SerializeString(VignetteName, 9, name: nameof(VignetteName));
            Unk3 = s.Serialize<uint>(Unk3, name: nameof(Unk3));
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Unk4 = s.Serialize<uint>(Unk4, name: nameof(Unk4));

            Data = s.SerializeArray<byte>(Data, Length, name: nameof(Data));
        }
    }
}