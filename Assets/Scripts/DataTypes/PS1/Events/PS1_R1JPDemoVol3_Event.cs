namespace R1Engine
{
    /// <summary>
    /// Event data for Rayman 1 (PS1 - Japan Demo Vol3)
    /// </summary>
    public class PS1_R1JPDemoVol3_Event : R1Serializable
    {
        // Graphics?
        public Pointer UnknownPointer1 { get; set; }
        public Pointer UnknownPointer2 { get; set; }
        public Pointer UnknownPointer3 { get; set; }

        // LabelOffsets?
        public Pointer UnknownPointer4 { get; set; }

        // Commands?
        public Pointer UnknownPointer5 { get; set; }

        public byte[] Unk1 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            UnknownPointer1 = s.SerializePointer(UnknownPointer1, name: nameof(UnknownPointer1));
            UnknownPointer2 = s.SerializePointer(UnknownPointer2, name: nameof(UnknownPointer2));
            UnknownPointer3 = s.SerializePointer(UnknownPointer3, name: nameof(UnknownPointer3));
            UnknownPointer4 = s.SerializePointer(UnknownPointer4, name: nameof(UnknownPointer4));
            UnknownPointer5 = s.SerializePointer(UnknownPointer5, name: nameof(UnknownPointer5));

            // Serialize values
            Unk1 = s.SerializeArray(Unk1, 124, name: nameof(Unk1));
        }
    }
}