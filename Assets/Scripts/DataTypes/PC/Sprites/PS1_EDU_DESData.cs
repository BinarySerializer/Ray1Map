namespace R1Engine
{
    /// <summary>
    /// DES data for EDU on PS1
    /// </summary>
    public class PS1_EDU_DESData : R1Serializable
    {
        public byte[] Unk1 { get; set; }

        // ushort/uint?
        public byte ImageDescriptorsCount { get; set; }

        public byte[] Unk2 { get; set; }

        // ushort/uint?
        public byte AnimationDescriptorsCount { get; set; }

        public byte[] Unk3 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk1 = s.SerializeArray<byte>(Unk1, 68, name: nameof(Unk1));
            ImageDescriptorsCount = s.Serialize<byte>(ImageDescriptorsCount, name: nameof(ImageDescriptorsCount));
            Unk2 = s.SerializeArray<byte>(Unk2, 63, name: nameof(Unk2));
            AnimationDescriptorsCount = s.Serialize<byte>(AnimationDescriptorsCount, name: nameof(AnimationDescriptorsCount));
            Unk3 = s.SerializeArray<byte>(Unk3, 3, name: nameof(Unk3));
        }
    }
}