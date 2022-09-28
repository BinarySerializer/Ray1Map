namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class AnimationChannel : BinarySerializable
    {
        public AnimationChannelType ChannelType { get; set; }
        
        // Sprite
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte SpriteShape { get; set; }
        public byte SpriteSize { get; set; }
        public bool UnknownFlag { get; set; }
        public bool IsAffine { get; set; }
        public ushort TileIndex { get; set; } // NOTE: Always a 4-bit index even if tileset is 8-bit
        public byte PalIndex { get; set; }
        public bool HorizontalFlip { get; set; }
        public bool VerticalFlip { get; set; }

        // Sound
        public ushort SoundIndex { get; set; }

        // Unknown
        public short Short_02 { get; set; }
        public short Short_04 { get; set; }

        // Box
        public sbyte Box_MinX { get; set; }
        public sbyte Box_MaxX { get; set; }
        public sbyte Box_MinY { get; set; }
        public sbyte Box_MaxY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b =>
            {
                YPosition = b.SerializeBits<short>(YPosition, 8, sign: SignedNumberRepresentation.TwosComplement, name: nameof(YPosition));
                IsAffine = b.SerializeBits<bool>(IsAffine, 1, name: nameof(IsAffine));
                UnknownFlag = b.SerializeBits<bool>(UnknownFlag, 1, name: nameof(UnknownFlag));
                ChannelType = b.SerializeBits<AnimationChannelType>(ChannelType, 4, name: nameof(ChannelType));
                SpriteShape = b.SerializeBits<byte>(SpriteShape, 2, name: nameof(SpriteShape));
            });

            if (ChannelType == AnimationChannelType.Sprite)
            {
                // Sprite channels mostly follow the GBA object attribute structure, however there are some changes such as
                // the positions being signed (since they're relative the absolute position) and values being replaced.

                s.DoBits<ushort>(b =>
                {
                    XPosition = b.SerializeBits<short>(XPosition, 9, sign: SignedNumberRepresentation.TwosComplement, name: nameof(XPosition));
                    PalIndex = b.SerializeBits<byte>(PalIndex, 3, name: nameof(PalIndex));

                    if (!IsAffine)
                    {
                        HorizontalFlip = b.SerializeBits<bool>(HorizontalFlip, 1, name: nameof(HorizontalFlip));
                        VerticalFlip = b.SerializeBits<bool>(VerticalFlip, 1, name: nameof(VerticalFlip));
                    }
                    else
                    {
                        b.SerializePadding(2, logIfNotNull: true);
                    }

                    SpriteSize = b.SerializeBits<byte>(SpriteSize, 2, name: nameof(SpriteSize));
                });
                TileIndex = s.Serialize<ushort>(TileIndex, name: nameof(TileIndex));
            }
            else if (ChannelType == AnimationChannelType.Sound)
            {
                SoundIndex = s.Serialize<ushort>(SoundIndex, name: nameof(SoundIndex));
                s.SerializePadding(2, logIfNotNull: true);
            }
            else if (ChannelType == AnimationChannelType.Unknown)
            {
                // Appears to be used to render something?

                Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
                Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
            }
            else if (ChannelType is AnimationChannelType.AttackBox or AnimationChannelType.VulnerabilityBox)
            {
                Box_MinY = s.Serialize<sbyte>(Box_MinY, name: nameof(Box_MinY));
                Box_MaxY = s.Serialize<sbyte>(Box_MaxY, name: nameof(Box_MaxY));
                Box_MinX = s.Serialize<sbyte>(Box_MinX, name: nameof(Box_MinX));
                Box_MaxX = s.Serialize<sbyte>(Box_MaxX, name: nameof(Box_MaxX));
            }
            else
            {
                throw new BinarySerializableException(this, $"Unsupported channel type {ChannelType}");
            }
        }
    }
}