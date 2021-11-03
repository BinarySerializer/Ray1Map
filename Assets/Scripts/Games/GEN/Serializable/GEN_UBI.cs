using System;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map.GEN
{
    public class GEN_UBI : BinarySerializable
    {
        public string USAF { get; set; }
        public string Version { get; set; }
        public byte Type { get; set; }
        public byte Unknown { get; set; }
        public ushort FrameSizeAudio { get; set; }
        public ushort FrameSizeSprite { get; set; }
        public ushort FramesCount { get; set; }
        public Frame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			USAF = s.SerializeString(USAF, 4, name: nameof(USAF));
            Version = s.SerializeString(Version, 2, name: nameof(Version));
            if(USAF != "USAF" || Version != "10")
                throw new NotImplementedException($"Unknown UBI Header {USAF}{Version}");

            Type = s.Serialize<byte>(Type, name: nameof(Type));
			Unknown = s.Serialize<byte>(Unknown, name: nameof(Unknown));
			FrameSizeAudio = s.Serialize<ushort>(FrameSizeAudio, name: nameof(FrameSizeAudio));
			FrameSizeSprite = s.Serialize<ushort>(FrameSizeSprite, name: nameof(FrameSizeSprite));
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			Frames = s.SerializeObjectArray<Frame>(Frames, FramesCount, onPreSerialize: c => c.UBI = this, name: nameof(Frames));
		}

		public class Frame : BinarySerializable {
            public GEN_UBI UBI { get; set; } // Set in onPreSerialize

            public byte[] AudioData { get; set; }
            public uint SpriteDataLength { get; set; }
            public UBI_SpriteData SpriteData { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				AudioData = s.SerializeArray<byte>(AudioData, UBI.FrameSizeAudio, name: nameof(AudioData));
                if (s.CurrentPointer.AbsoluteOffset >= s.CurrentLength) return;
				SpriteDataLength = s.Serialize<uint>(SpriteDataLength, name: nameof(SpriteDataLength));
                if (SpriteDataLength > 0) {
                    Pointer Current = s.CurrentPointer;
                    SpriteData = s.SerializeObject<UBI_SpriteData>(SpriteData, name: nameof(SpriteData));
                    if (s.CurrentPointer != Current + SpriteDataLength) {
                        UnityEngine.Debug.LogWarning($"{Offset}: Data length was {(s.CurrentPointer - Current):X8}, not {SpriteDataLength:X8}");
                        s.Goto(Current + SpriteDataLength);
                    }
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
                public Type SectionType { get; set; }
                public uint Length { get; set; }
                public byte[] SectionData { get; set; }
                public GEN_RLX RLX { get; set; }
                public BaseColor[] Palette { get; set; }

                public override void SerializeImpl(SerializerObject s) {
                    SectionType = s.Serialize<Type>(SectionType, name: nameof(SectionType));
                    Length = s.Serialize<uint>(Length, name: nameof(Length));
                    if (Length > 0) {
                        switch (SectionType) {
                            case Type.RLX_Sprite:
                                RLX = s.SerializeObject<GEN_RLX>(RLX, rlx => rlx.FileSize = Length, name: nameof(RLX));
                                break;
                            case Type.Palette_8:
                                Palette = s.SerializeObjectArray<RGB888Color>((RGB888Color[])Palette, Math.Min(Length / 3, 256), name: nameof(Palette));
                                break;
                            case Type.RLX_BackgroundPart:
                                RLX = s.SerializeObject<GEN_RLX>(RLX, rlx => rlx.FileSize = Length, name: nameof(RLX));
                                break;
                            case Type.Palette_15:
                            case Type.Palette_20:
                                SectionData = s.SerializeArray<byte>(SectionData, 0x18, name: nameof(SectionData));
                                Palette = s.SerializeObjectArray<RGBA8888Color>((RGBA8888Color[])Palette, Math.Min(Length / 4, 256), name: nameof(Palette));
                                break;
                            default:
                                SectionData = s.SerializeArray<byte>(SectionData, Length, name: nameof(SectionData));
                                break;
                        }
                    }
                }

                public enum Type : byte {
                    RLX_Sprite = 3,
                    Palette_8 = 8, // 888
                    PushBackgroundLayer = 9, // write to texture
                    RLX_BackgroundPart = 10,
                    Palette_15 = 15, // 8888 with a RIFF header that can be garbage data
                    Palette_20 = 20, // 8888 with a RIFF header that can be garbage data
                }
			}
		}
	}
}