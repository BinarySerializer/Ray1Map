namespace R1Engine
{
    /// <summary>
    /// Event graphics block for some special events in Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventGraphicsReference : R1Serializable
    {
        public ushort StructType { get; set; } // Read from EventDefinition

        public Pointer GraphicsPointer { get; set; }
        public Pointer CodePointer1 { get; set; }
        public Pointer CodePointer2 { get; set; }
        public byte[] UnkBytes { get; set; }

        // Parsed
        public Jaguar_R1_EventGraphics Graphics { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsPointer = s.SerializePointer(GraphicsPointer, name: nameof(GraphicsPointer));
            CodePointer1 = s.SerializePointer(CodePointer1, name: nameof(CodePointer1));
            CodePointer2 = s.SerializePointer(CodePointer2, name: nameof(CodePointer2));
            UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x14, name: nameof(UnkBytes));

            s.DoAt(GraphicsPointer, () => {
                Graphics = s.SerializeObject<Jaguar_R1_EventGraphics>(Graphics, onPreSerialize: g => g.StructType = StructType, name: nameof(Graphics));
            });
        }
    }
}