namespace R1Engine
{
    public class GBAIsometric_Spyro_LocBlock : R1Serializable
    {
        // Set in onPreSerialize
        public int Length { get; set; }
        public GBAIsometric_Spyro_LocDecompress[] DecompressHelpers { get; set; }

        public ushort[] StringOffsets { get; set; }
        public byte[][] StringTileIndices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringOffsets = s.SerializeArray<ushort>(StringOffsets, Length, name: nameof(StringOffsets));
            if (StringTileIndices == null) {
                StringTileIndices = new byte[StringOffsets.Length][];
                for (int i = 0; i < StringOffsets.Length; i++) {
                    s.DoAt(Offset + StringOffsets[i], () => {
                        s.DoEncoded(new Spyro_StringEncoder(DecompressHelpers), () => {
                            //s.SerializeString(default, length: s.CurrentLength, name: "String");
                            StringTileIndices[i] = s.SerializeArray<byte>(StringTileIndices[i], s.CurrentLength, name: $"{nameof(StringTileIndices)}[{i}]");
                            // Add 46 to each byte to get good lowercase characters
                        });
                    });
                }
            }
        }
    }
}