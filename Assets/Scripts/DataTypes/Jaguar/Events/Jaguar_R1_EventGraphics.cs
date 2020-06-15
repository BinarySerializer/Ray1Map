namespace R1Engine
{
    /// <summary>
    /// Event graphics block for some special events in Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventGraphics : R1Serializable
    {
        public ushort StructType { get; set; } // Read from EventDefinition

        public byte[] UnkBytes { get; set; }
        public Pointer ImageDescriptorsPointer { get; set; }
        public Jaguar_R1_EventGraphicsReference[] GraphicsReferences { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (StructType != 29) {
                UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x10, name: nameof(UnkBytes));
            }
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            if (StructType != 29) {
                GraphicsReferences = s.SerializeObjectArray<Jaguar_R1_EventGraphicsReference>(GraphicsReferences, 7, onPreSerialize: g => g.StructType = StructType, name: nameof(GraphicsReferences));
            }
            // TODO: After this: states (struct is a bit different, and animation pointers point to first layer instead of header, so do pointer - 0x4 to get the header)
        }
    }
}