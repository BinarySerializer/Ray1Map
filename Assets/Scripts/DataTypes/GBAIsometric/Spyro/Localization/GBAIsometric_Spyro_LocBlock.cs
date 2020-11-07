namespace R1Engine
{
    public class GBAIsometric_Spyro_LocBlock : R1Serializable
    {
        // Set in onPreSerialize
        public int Length { get; set; }
        public GBAIsometric_Spyro_LocDecompress[] DecompressHelpers { get; set; }

        public ushort[] StringOffsets { get; set; }
        public byte[][] StringTileIndices { get; set; }
        public string[] Strings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringOffsets = s.SerializeArray<ushort>(StringOffsets, Length, name: nameof(StringOffsets));
            if (StringTileIndices == null) 
            {
                StringTileIndices = new byte[StringOffsets.Length][];
                Strings = new string[StringOffsets.Length];

                var encoding = new SpyroEncoding(s.GameSettings.GameModeSelection);

                for (int i = 0; i < StringOffsets.Length; i++) {
                    s.DoAt(Offset + StringOffsets[i], () => {
                        s.DoEncoded(new Spyro_StringEncoder(DecompressHelpers), () => {
                            StringTileIndices[i] = s.SerializeArray<byte>(StringTileIndices[i], s.CurrentLength, name: $"{nameof(StringTileIndices)}[{i}]");

                            var str = encoding.GetString(StringTileIndices[i]);
                            Strings[i] = str;
                            s.Log($"String: {str}");
                        });
                    });
                }
            }
        }
    }
}