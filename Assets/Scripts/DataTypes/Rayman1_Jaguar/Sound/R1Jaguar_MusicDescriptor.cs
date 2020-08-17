namespace R1Engine
{
    /// <summary>
    /// Music data for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_MusicDescriptor : R1Serializable
    {
        public Pointer MusicDataPointer { get; set; }
        public ushort UShort_04 { get; set; }
        public short Short_06 { get; set; }
        public byte[] Unk { get; set; }

        // Parsed
        public R1Jaguar_MusicData[] MusicData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            MusicDataPointer = s.SerializePointer(MusicDataPointer, name: nameof(MusicDataPointer));
            UShort_04 = s.Serialize<ushort>(UShort_04, name: nameof(UShort_04));
            Short_06 = s.Serialize<short>(Short_06, name: nameof(Short_06));
            Unk = s.SerializeArray<byte>(Unk, 8, name: nameof(Unk));

            s.DoAt(MusicDataPointer, () => {
                s.DoEncoded(new RNCEncoder(), () => {
                    MusicData = s.SerializeObjectArray(MusicData, s.CurrentLength / 0x8, name: nameof(MusicData));
                });
            });
        }
    }
}