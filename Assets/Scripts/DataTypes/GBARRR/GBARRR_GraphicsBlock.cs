namespace R1Engine
{
    public class GBARRR_GraphicsBlock : R1Serializable
    {
        public uint Count { get; set; }
        public uint TileSize { get; set; }

        public Header[] Headers { get; set; }
        public byte[][] TileData { get; set; }
        public byte[][] TileDataDecompr { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Headers = s.SerializeObjectArray<Header>(Headers, Count+1, name: nameof(Headers));
            if (TileData == null) {
                TileData = new byte[Count][];
                for (int i = 0; i < Count; i++) {
                    s.DoAt(Offset + Headers[i].TileOffset, () => {
                        int length = (Headers[i + 1].TileOffset - Headers[i].TileOffset - Headers[i].ExtraBytes);

                        if (length != TileSize * TileSize / 2) {
                            s.DoEncoded(new LZSSEncoder((uint)length, hasHeader: false), () => {
                                TileData[i] = s.SerializeArray<byte>(TileData[i], s.CurrentLength, name: $"{nameof(TileData)}[{i}]");
                            });
                        } else {
                            TileData[i] = s.SerializeArray<byte>(TileData[i], length, name: $"{nameof(TileData)}[{i}]");
                        }
                    });
                }
            }
        }

        public class Header : R1Serializable {
            public int TileOffset { get; set; }
            public int ExtraBytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.SerializeBitValues<uint>((bitFunc) => {
                    TileOffset = bitFunc(TileOffset, 24, name: nameof(TileOffset));
                    ExtraBytes = bitFunc(ExtraBytes, 8, name: nameof(ExtraBytes));
                });
			}
		}
    }
}