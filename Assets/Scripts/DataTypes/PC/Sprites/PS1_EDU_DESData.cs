namespace R1Engine
{
    // This class uses the same structure as normal events. Dummy data is always 0 as the event values aren't used here.

    /// <summary>
    /// DES data for EDU on PS1
    /// </summary>
    public class PS1_EDU_DESData : R1Serializable
    {
        public uint ImageDescriptorsPointer { get; set; }

        public uint AnimationDescriptorsPointer { get; set; }

        public byte[] Dummy1 { get; set; }

        public uint ImageBufferLength { get; set; }

        public byte[] Dummy2 { get; set; }

        public ushort ImageDescriptorsCount { get; set; }

        public byte[] Dummy3 { get; set; }

        public byte AnimationDescriptorsCount { get; set; }

        public byte[] Dummy4 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            ImageDescriptorsPointer = s.Serialize<uint>(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            AnimationDescriptorsPointer = s.Serialize<uint>(AnimationDescriptorsPointer, name: nameof(AnimationDescriptorsPointer));
            Dummy1 = s.SerializeArray<byte>(Dummy1, 32, name: nameof(Dummy1));
            ImageBufferLength = s.Serialize<uint>(ImageBufferLength, name: nameof(ImageBufferLength));
            Dummy2 = s.SerializeArray<byte>(Dummy2, 24, name: nameof(Dummy2));
            ImageDescriptorsCount = s.Serialize<ushort>(ImageDescriptorsCount, name: nameof(ImageDescriptorsCount));
            Dummy3 = s.SerializeArray<byte>(Dummy3, 62, name: nameof(Dummy3));
            AnimationDescriptorsCount = s.Serialize<byte>(AnimationDescriptorsCount, name: nameof(AnimationDescriptorsCount));
            Dummy4 = s.SerializeArray<byte>(Dummy4, 3, name: nameof(Dummy4));
        }
    }
}