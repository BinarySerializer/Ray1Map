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

        // Donald Duck
        public TileMapInfo DD_Map_TileIndices { get; set; }
        public byte DD_Map_Width { get; set; }
        public byte DD_Map_Height { get; set; }

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

                case InstructionCommand.SetMapDimensions:
                    DD_Map_Width = s.Serialize<byte>(DD_Map_Width, name: nameof(DD_Map_Width));
                    DD_Map_Height = s.Serialize<byte>(DD_Map_Height, name: nameof(DD_Map_Height));
                    break;
                case InstructionCommand.SetMapGraphics:
                    DD_Map_TileIndices = s.SerializeObject<TileMapInfo>(DD_Map_TileIndices, name: nameof(DD_Map_TileIndices));
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

            SetMapDimensions = 0x1D,
            SetMapGraphics = 0x1E,

            Terminator = 0xFF,
        }

        public class TileAttribute : R1Serializable {
            public byte PalIndex { get; set; }
            public byte Unknown1 { get; set; }
            public bool HorizontalFlip { get; set; }
            public bool VerticalFlip { get; set; }
            public bool Unknown2 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                s.SerializeBitValues<byte>(bitFunc => {
                    PalIndex = (byte)bitFunc(PalIndex, 3, name: nameof(PalIndex));
                    Unknown1 = (byte)bitFunc(Unknown1, 2, name: nameof(Unknown1));
                    HorizontalFlip = bitFunc(HorizontalFlip ? 1 : 0, 1, name: nameof(HorizontalFlip)) == 1;
                    VerticalFlip = bitFunc(VerticalFlip ? 1 : 0, 1, name: nameof(VerticalFlip)) == 1;
                    Unknown2 = bitFunc(Unknown2 ? 1 : 0, 1, name: nameof(Unknown2)) == 1;
                });
            }
        }

        public class TileGraphicsInfo : R1Serializable
        {
            public byte TileIndex { get; set; } // If -1, same effect as SetInvisible, otherwise SetVisible
            public TileAttribute Attribute { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TileIndex = s.Serialize<byte>(TileIndex, name: nameof(TileIndex));
                Attribute = s.SerializeObject<TileAttribute>(Attribute, name: nameof(Attribute));
            }
        }

        public class LayerInfo : R1Serializable
        {
            public byte DrawIndex { get; set; } // The index this sprite is given in the puppet's sprite array
            public TileGraphicsInfo Tile { get; set; }
            public sbyte XPos { get; set; }
            public sbyte YPos { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                DrawIndex = s.Serialize<byte>(DrawIndex, name: nameof(DrawIndex));
                Tile = s.SerializeObject<TileGraphicsInfo>(Tile, name: nameof(Tile));
                XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
                YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
            }
        }

        public class TileMapInfo : R1Serializable {
            public byte Byte00_Indices { get; set; }
            public byte Count_Indices { get; set; }
            public byte[] TileIndices { get; set; }
            public byte LastByte_Indices { get; set; }
            public byte Byte00_Attributes { get; set; }
            public byte Count_Attributes { get; set; }
            public TileAttribute[] Attributes { get; set; }
            public byte LastByte_Attributes { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                Byte00_Indices = s.Serialize<byte>(Byte00_Indices, name: nameof(Byte00_Indices));
                Count_Indices = s.Serialize<byte>(Count_Indices, name: nameof(Count_Indices));
				TileIndices = s.SerializeArray<byte>(TileIndices, Count_Indices, name: nameof(TileIndices));
                LastByte_Indices = s.Serialize<byte>(LastByte_Indices, name: nameof(LastByte_Indices));
                Byte00_Attributes = s.Serialize<byte>(Byte00_Attributes, name: nameof(Byte00_Attributes));
                Count_Attributes = s.Serialize<byte>(Count_Attributes, name: nameof(Count_Attributes));
                Attributes = s.SerializeObjectArray<TileAttribute>(Attributes, Count_Attributes, name: nameof(Attributes));
                LastByte_Attributes = s.Serialize<byte>(LastByte_Attributes, name: nameof(LastByte_Attributes));
            }
		}
    }
}