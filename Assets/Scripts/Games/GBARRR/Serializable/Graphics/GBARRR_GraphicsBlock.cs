using System;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBARRR
{
    public class GBARRR_GraphicsBlock : BinarySerializable
    {
        public uint Count { get; set; }
        public uint TileSize { get; set; }

        public Header[] Headers { get; set; }
        public byte[][] TileData { get; set; }

        // Custom
        public int BlockIndex { get; set; }
        public GBA_RRR_Manager.AnimationAssemble AnimationAssemble { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            bool determineTileSize = false;
            if (Count == 0) {
                Headers = s.SerializeObjectArray<Header>(Headers, 2, name: nameof(Headers));

                if(Headers[0].TileOffset % 4 != 0 || Headers[0].ExtraBytes >= 4 || (Headers[0].TileOffset / 4) < 2) return; // Invalid
                Count = (uint)(Headers[0].TileOffset / 4) - 1;
                determineTileSize = true;
                s.Goto(Offset);
            }
            Headers = s.SerializeObjectArray<Header>(Headers, Count + 1, name: nameof(Headers));
            if (TileData == null) {
                TileData = new byte[Count][];
                for (int i = 0; i < Count; i++) {
                    s.DoAt(Offset + Headers[i].TileOffset, () => {
                        int length = (Headers[i + 1].TileOffset - Headers[i].TileOffset - Headers[i].ExtraBytes);

                        if (determineTileSize && i == 0) {
                            if (Math.Sqrt(length * 2) % 1 == 0) {
                                int val = Mathf.RoundToInt(Mathf.Sqrt(length * 2));
                                if ((val != 0) && ((val & (val - 1)) == 0)) {
                                    TileSize = (uint)val;
                                }
                            }
                        }

                        if (length != TileSize * TileSize / 2) {
                            s.DoEncoded(new LZSSEncoder((uint)length, hasHeader: false), () => {
                                TileData[i] = s.SerializeArray<byte>(TileData[i], s.CurrentLength, name: $"{nameof(TileData)}[{i}]");
                            });
                        } else {
                            TileData[i] = s.SerializeArray<byte>(TileData[i], length, name: $"{nameof(TileData)}[{i}]");
                        }
                        if (determineTileSize && i == 0) {
                            int len = TileData[i].Length;
                            if (Math.Sqrt(len * 2) % 1 == 0) {
                                TileSize = (uint)Mathf.RoundToInt(Mathf.Sqrt(len * 2));
                            }
                        }
                    });
                }
            }
        }

        public class Header : BinarySerializable {
            public int TileOffset { get; set; }
            public int ExtraBytes { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				s.DoBits<uint>(b => {
                    TileOffset = b.SerializeBits<int>(TileOffset, 24, name: nameof(TileOffset));
                    ExtraBytes = b.SerializeBits<int>(ExtraBytes, 8, name: nameof(ExtraBytes));
                });
			}
		}
    }
}