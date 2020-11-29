using System;

namespace R1Engine
{
    public class GBC_Keyframe_Command : R1Serializable {
        public GBC_RomChannel ChannelData { get; set; } // Set before serializing

        public InstructionCommand Command { get; set; }

        // Params
        public byte ChannelIndex { get; set; }
        
        public byte LayerInfosCount { get; set; }
        public LayerInfo[] LayerInfos { get; set; }

        public sbyte XPos { get; set; }
        public sbyte YPos { get; set; }

        public byte HalfHeight { get; set; }
        public byte HalfWidth { get; set; }

        public byte UnkHitboxValue { get; set; }
        
        public TileGraphicsInfo[] TileGraphicsInfos { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Command = s.Serialize<InstructionCommand>(Command, name: nameof(Command));

            switch (Command)
            {
                case InstructionCommand.SpriteNew:
                    LayerInfosCount = s.Serialize<byte>(LayerInfosCount, name: nameof(LayerInfosCount));
                    LayerInfos = s.SerializeObjectArray<LayerInfo>(LayerInfos, LayerInfosCount, name: nameof(LayerInfos));
                    ChannelData?.Temp_LayerSpriteCountState?.Add(LayerInfosCount);
                    break;

                case InstructionCommand.SpriteMove:
                    ChannelIndex = s.Serialize<byte>(ChannelIndex, name: nameof(ChannelIndex));
                    XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
                    break;

                case InstructionCommand.SetTileGraphics:
                    ChannelIndex = s.Serialize<byte>(ChannelIndex, name: nameof(ChannelIndex));
                    TileGraphicsInfos = s.SerializeObjectArray<TileGraphicsInfo>(TileGraphicsInfos, TileGraphicsInfos?.Length ?? ChannelData?.Temp_LayerSpriteCountState[ChannelIndex] ?? 0, name: nameof(TileGraphicsInfos));
                    break;

                case InstructionCommand.SetCollisionBox:
                    XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
                    HalfWidth = s.Serialize<byte>(HalfWidth, name: nameof(HalfWidth));
                    HalfHeight = s.Serialize<byte>(HalfHeight, name: nameof(HalfHeight));
                    break;

                case InstructionCommand.SetInvisible: // Set sprite->field6 to 0 for all sprites in layer
                case InstructionCommand.SetVisible: // Set sprite->field6 to 1 for all sprites in layer
                    ChannelIndex = s.Serialize<byte>(ChannelIndex, name: nameof(ChannelIndex));
                    break;

                case InstructionCommand.Unknown_0C:
                case InstructionCommand.Unknown_0D:
                case InstructionCommand.Unknown_0E:
                    UnkHitboxValue = s.Serialize<byte>(UnkHitboxValue, name: nameof(UnkHitboxValue));
                    break;

                case InstructionCommand.Terminator:
                    ChannelData?.Temp_LayerSpriteCountState?.Clear();
                    break;

                case InstructionCommand.SpriteDelete:
                default:
                    throw new ArgumentOutOfRangeException(nameof(Command), Command, null);
            }
        }

        public enum InstructionCommand : byte
        {
            SpriteNew = 0x01,
            SpriteDelete = 0x02,
            
            SpriteMove = 0x03,
            //SpriteMoveMultiple = 0x04,
            SetTileGraphics = 0x05,

            SetInvisible = 0x07,
            SetVisible = 0x08,

            SetCollisionBox = 0x0B,
            Unknown_0C = 0x0C,
            Unknown_0D = 0x0D,
            Unknown_0E = 0x0E,

            Terminator = 0xFF,
        }

        public class TileGraphicsInfo : R1Serializable
        {
            public byte TileIndex { get; set; } // If -1, same effect as SetInvisible, otherwise SetVisible
            public byte Attr_PalIndex { get; set; }
            public byte Attr_VRAMBank { get; set; }
            public bool Attr_HorizontalFlip { get; set; }
            public bool Attr_VerticalFlip { get; set; }
            public bool Attr_Prio { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileIndex = s.Serialize<byte>(TileIndex, name: nameof(TileIndex));
                s.SerializeBitValues<byte>(bitFunc =>
                {
                    Attr_PalIndex = (byte)bitFunc(Attr_PalIndex, 2, name: nameof(Attr_PalIndex));
                    Attr_VRAMBank = (byte)bitFunc(Attr_VRAMBank, 1, name: nameof(Attr_VRAMBank));
                    bitFunc(default, 1, name: "Attr_Padding"); // Palette number for non-CGB mode, which is irrelevant here
                    Attr_VerticalFlip = bitFunc(Attr_VerticalFlip ? 1 : 0, 1, name: nameof(Attr_VerticalFlip)) == 1;
                    Attr_HorizontalFlip = bitFunc(Attr_HorizontalFlip ? 1 : 0, 1, name: nameof(Attr_HorizontalFlip)) == 1;
                    Attr_Prio = bitFunc(Attr_Prio ? 1 : 0, 1, name: nameof(Attr_Prio)) == 1;
                });
            }
        }

        public class LayerInfo : R1Serializable
        {
            public byte SpriteID { get; set; } // The index this sprite is given in the puppet's sprite array
            public TileGraphicsInfo Tile { get; set; }
            public sbyte XPos { get; set; }
            public sbyte YPos { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                SpriteID = s.Serialize<byte>(SpriteID, name: nameof(SpriteID));
                Tile = s.SerializeObject<TileGraphicsInfo>(Tile, name: nameof(Tile));
                XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
                YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
            }
        }
    }
}