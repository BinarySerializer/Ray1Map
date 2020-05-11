namespace R1Engine
{
    /// <summary>
    /// Unknown struct for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_UnkStruct : R1Serializable
    {
        public Pointer Pointer_00 { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0B { get; set; }

        public ushort UShort_10 { get; set; }
        public ushort UShort_12 { get; set; }

        public byte[] UnkBytes_14 { get; set; }

        public Pointer Pointer_18 { get; set; }
        public Pointer Pointer_1B { get; set; }

        public byte[] UnkBytes_20 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializePointer(Pointer_00, name: nameof(Pointer_00));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0B = s.SerializePointer(Pointer_0B, name: nameof(Pointer_0B));

            UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
            UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));

            UnkBytes_14 = s.SerializeArray<byte>(UnkBytes_14, 4, name: nameof(UnkBytes_14));

            Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));
            Pointer_1B = s.SerializePointer(Pointer_1B, name: nameof(Pointer_1B));

            UnkBytes_20 = s.SerializeArray<byte>(UnkBytes_20, 4, name: nameof(UnkBytes_20));
        }
    }
}