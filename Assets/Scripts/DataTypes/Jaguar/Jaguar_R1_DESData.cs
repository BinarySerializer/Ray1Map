namespace R1Engine
{
    /// <summary>
    /// DES data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_DESData : R1Serializable
    {
        public ushort UShort_00 { get; set; }
        public Pointer Pointer_02 { get; set; }
        public ushort UShort_06 { get; set; }

        // Sometimes a pointer, sometimes not. Why? Until we figure it out, serialize all pointers as uints
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0C { get; set; }
        public Pointer Pointer_10 { get; set; }

        // Pointer to image descriptors
        public Pointer Pointer_14 { get; set; }

        public uint UInt_08 { get; set; }
        public uint UInt_0C { get; set; }
        public uint UInt_10 { get; set; }
        public uint UInt_14 { get; set; }

        public uint UInt_18 { get; set; }
        public uint UInt_1C { get; set; }
        public ushort UShort_20 { get; set; }
        public uint UInt_22 { get; set; }
        public Pointer Pointer_22 { get; set; }
        public ushort UShort_26 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            Pointer_02 = s.SerializePointer(Pointer_02, name: nameof(Pointer_02));
            UShort_06 = s.Serialize<ushort>(UShort_06, name: nameof(UShort_06));
            UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
            UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
            UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
            UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
            UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
            UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
            UShort_20 = s.Serialize<ushort>(UShort_20, name: nameof(UShort_20));
            UInt_22 = s.Serialize<uint>(UInt_22, name: nameof(UInt_22));
            UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));
        }
    }
}