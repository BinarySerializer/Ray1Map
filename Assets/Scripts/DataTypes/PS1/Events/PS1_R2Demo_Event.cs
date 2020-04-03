namespace R1Engine
{
    // TODO: Merge with PS1_R1_Event once all values have been parsed
    /// <summary>
    /// Event data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Event : R1Serializable
    {
        public Pointer UnkPointer1 { get; set; }

        public Pointer UnkPointer2 { get; set; }

        public Pointer UnkPointer3 { get; set; }

        public byte[] Unk1 { get; set; }

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

            Unk1 = s.SerializeArray(Unk1, 96, name: nameof(Unk1));

            //if (UnkPointer1 != null)
            //{
            //    s.DoAt(UnkPointer1, () =>
            //    {
            //        s.SerializeArray(new byte[0], 100, name: "UnkPointer1Data");
            //    });
            //}
            //if (UnkPointer2 != null)
            //{
            //    s.DoAt(UnkPointer2, () =>
            //    {
            //        s.SerializeArray(new byte[0], 100, name: "UnkPointer2Data");
            //    });
            //}
            //if (UnkPointer3 != null)
            //{
            //    s.DoAt(UnkPointer3, () =>
            //    {
            //        s.SerializeArray(new byte[0], 100, name: "UnkPointer3Data");
            //    });
            //}
        }
    }
}