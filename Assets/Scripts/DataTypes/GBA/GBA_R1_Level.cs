namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_Level : R1Serializable
    {
        public Pointer Pointer_00 { get; set; }
        public Pointer Pointer_04 { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0B { get; set; }

        public byte[] Unk_10 { get; set; }

        // Some world-specific info (tiles, palettes?)
        public Pointer WorldPointer { get; set; }

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
            Unk_10 = s.SerializeArray<byte>(Unk_10, 8, name: nameof(Unk_10));
            WorldPointer = s.SerializePointer(WorldPointer, name: nameof(WorldPointer));
        }
    }
}