namespace R1Engine
{
    /// <summary>
    /// Event state for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventComplexDataState : R1Serializable
    {
        public Pointer AnimationPointer { get; set; }
        public byte Byte04 { get; set; }
        public byte Byte05 { get; set; }
        public byte Byte06 { get; set; }
        public byte LinkedStateIndex { get; set; }
        public byte[] UnkBytes { get; set; }

        // Parsed
        public Jaguar_R1_AnimationDescriptor Animation { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));
            Byte04 = s.Serialize<byte>(Byte04, name: nameof(Byte04));
            Byte05 = s.Serialize<byte>(Byte05, name: nameof(Byte05));
            Byte06 = s.Serialize<byte>(Byte06, name: nameof(Byte06));
            LinkedStateIndex = s.Serialize<byte>(LinkedStateIndex, name: nameof(LinkedStateIndex));
            UnkBytes = s.SerializeArray<byte>(UnkBytes, 8, name: nameof(UnkBytes));

            if (AnimationPointer != null) {
                // AnimationPointer points to first layer. So, go back 4 bytes to get header
                s.DoAt(AnimationPointer - 0x4, () => {
                    Animation = s.SerializeObject<Jaguar_R1_AnimationDescriptor>(Animation, name: nameof(Animation));
                });
            }
        }
    }
}