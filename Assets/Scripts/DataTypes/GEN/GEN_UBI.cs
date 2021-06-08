using System;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine
{
    public class GEN_UBI : BinarySerializable
    {
        public string USAF { get; set; }
        public string Version { get; set; }
        public byte Type { get; set; }
        public byte Unknown { get; set; }
        public ushort ChunkSizeAudio { get; set; }
        public ushort ChunkSizeSprite { get; set; }
        public ushort ChunksCount { get; set; }
        public Chunk[] Chunks { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			USAF = s.SerializeString(USAF, 4, name: nameof(USAF));
            Version = s.SerializeString(Version, 2, name: nameof(Version));
            if(USAF != "USAF" || Version != "10")
                throw new NotImplementedException($"Unknown UBI Header {USAF}{Version}");

            Type = s.Serialize<byte>(Type, name: nameof(Type));
			Unknown = s.Serialize<byte>(Unknown, name: nameof(Unknown));
			ChunkSizeAudio = s.Serialize<ushort>(ChunkSizeAudio, name: nameof(ChunkSizeAudio));
			ChunkSizeSprite = s.Serialize<ushort>(ChunkSizeSprite, name: nameof(ChunkSizeSprite));
			ChunksCount = s.Serialize<ushort>(ChunksCount, name: nameof(ChunksCount));
			Chunks = s.SerializeObjectArray<Chunk>(Chunks, ChunksCount, onPreSerialize: c => c.UBI = this, name: nameof(Chunks));
		}

		public class Chunk : BinarySerializable {
            public GEN_UBI UBI { get; set; } // Set in onPreSerialize

            public byte[] AudioData { get; set; }
            public uint SpriteDataLength { get; set; }
            public UBI_SpriteData SpriteData { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				AudioData = s.SerializeArray<byte>(AudioData, UBI.ChunkSizeAudio, name: nameof(AudioData));
                if (s.CurrentPointer.AbsoluteOffset >= s.CurrentLength) return;
				SpriteDataLength = s.Serialize<uint>(SpriteDataLength, name: nameof(SpriteDataLength));
                if (SpriteDataLength > 0) {
                    Pointer Current = s.CurrentPointer;
                    SpriteData = s.SerializeObject<UBI_SpriteData>(SpriteData, name: nameof(SpriteData));
                    if (s.CurrentPointer != Current + SpriteDataLength)
                        throw new BinarySerializableException(this, $"Data length was {(s.CurrentPointer - Current):X8}, not {SpriteDataLength:X8}");
                }
			}
		}

		public class UBI_SpriteData : BinarySerializable {
            public byte Count { get; set; }
            public Section[] Sections { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                Count = s.Serialize<byte>(Count, name: nameof(Count));
				Sections = s.SerializeObjectArray<Section>(Sections, Count, name: nameof(Sections));
			}

            public class Section : BinarySerializable {
                public byte Type { get; set; }
                public uint Length { get; set; }
                public byte[] Data { get; set; }

                public override void SerializeImpl(SerializerObject s) {
                    Type = s.Serialize<byte>(Type, name: nameof(Type));
                    Length = s.Serialize<uint>(Length, name: nameof(Length));
                    Data = s.SerializeArray<byte>(Data, Length, name: nameof(Data));
                }
			}
		}
	}
}