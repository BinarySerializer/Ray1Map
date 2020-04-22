namespace R1Engine
{
    // TODO: Merge with PS1_R1_Event once all values have been parsed
    /// <summary>
    /// Event data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Event : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; }

        // Leads to 16-byte long structures
        public Pointer UnkPointer2 { get; set; }

        // Leads to 12-byte long structures. Usually it's {Pointer1, Pointer2, ushort1, ushort2}
        // Pointer1 leads to more pointers
        public Pointer UnkPointer3 { get; set; }

        // Always 0
        public uint Unk1 { get; set; }

        public ushort XPosition { get; set; }
        
        public ushort YPosition { get; set; }

        public byte[] Unk2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
            UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));
            UnkPointer3 = s.SerializePointer(UnkPointer3, name: nameof(UnkPointer3));

            Unk1 = s.Serialize<uint>(Unk1, name: nameof(Unk1));
            XPosition = s.Serialize<ushort>(XPosition, name: nameof(XPosition));
            YPosition = s.Serialize<ushort>(YPosition, name: nameof(YPosition));

            Unk2 = s.SerializeArray(Unk2, 88, name: nameof(Unk2));
        }
    }
}