namespace R1Engine
{
    /// <summary>
    /// DES data for EDU on PS1
    /// </summary>
    public class PS1_EDU_DESData : R1Serializable
    {
        public uint ImageDescriptorsPointer { get; set; }

        public uint AnimationDescriptorsPointer { get; set; }

        // Always 0?
        public byte[] Unk1 { get; set; }

        public uint ImageBufferLength { get; set; }

        // Always 0?
        public byte[] Unk2 { get; set; }

        public uint ImageDescriptorsCount { get; set; }

        // Always 0?
        public byte[] Unk3 { get; set; }

        public uint AnimationDescriptorsCount { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ImageDescriptorsPointer = s.Serialize<uint>(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            AnimationDescriptorsPointer = s.Serialize<uint>(AnimationDescriptorsPointer, name: nameof(AnimationDescriptorsPointer));
            Unk1 = s.SerializeArray<byte>(Unk1, 32, name: nameof(Unk1));
            ImageBufferLength = s.Serialize<uint>(ImageBufferLength, name: nameof(ImageBufferLength));
            Unk2 = s.SerializeArray<byte>(Unk2, 24, name: nameof(Unk2));
            ImageDescriptorsCount = s.Serialize<uint>(ImageDescriptorsCount, name: nameof(ImageDescriptorsCount));
            Unk3 = s.SerializeArray<byte>(Unk3, 60, name: nameof(Unk3));
            AnimationDescriptorsCount = s.Serialize<uint>(AnimationDescriptorsCount, name: nameof(AnimationDescriptorsCount));
        }
    }
}