using System;

namespace R1Engine
{
    public class GBC_ChannelEventInstruction : R1Serializable
    {
        public InstructionCommand Command { get; set; }

        // Params
        public byte LayerIndex { get; set; }
        
        public sbyte XPos { get; set; }
        public sbyte YPos { get; set; }
        
        public TileGraphicsInfo[] TileGraphicsInfos { get; set; }

        public byte[] UnknownParams { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Command = s.Serialize<InstructionCommand>(Command, name: nameof(Command));

            switch (Command)
            {
                case InstructionCommand.SetTilePosition:
                    LayerIndex = s.Serialize<byte>(LayerIndex, name: nameof(LayerIndex));
                    XPos = s.Serialize<sbyte>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<sbyte>(YPos, name: nameof(YPos));
                    break;

                case InstructionCommand.SetTileGraphics:
                    LayerIndex = s.Serialize<byte>(LayerIndex, name: nameof(LayerIndex));
                    TileGraphicsInfos = s.SerializeObjectArray<TileGraphicsInfo>(TileGraphicsInfos, 1, name: nameof(TileGraphicsInfos)); // TODO: Get count
                    break;

                case InstructionCommand.Unknown_0B: // Hitbox? x, y, w, h?
                    UnknownParams = s.SerializeArray<byte>(UnknownParams, 4, name: nameof(UnknownParams));
                    break;

                case InstructionCommand.Unknown_01:
                case InstructionCommand.Unknown_02:
                case InstructionCommand.Unknown_07:
                case InstructionCommand.Unknown_08:
                case InstructionCommand.Unknown_0C:
                    UnknownParams = s.SerializeArray<byte>(UnknownParams, 1, name: nameof(UnknownParams));
                    break;

                case InstructionCommand.Terminator:
                    // Do nothing
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Command), Command, null);
            }
        }

        public enum InstructionCommand : byte
        {
            Unknown_01 = 0x01,
            Unknown_02 = 0x02,
            
            SetTilePosition = 0x03,
            SetTileGraphics = 0x05,

            Unknown_07 = 0x07,
            Unknown_08 = 0x08,

            Unknown_0B = 0x0B,
            Unknown_0C = 0x0C,
            Unknown_0D = 0x0D,
            Unknown_0E = 0x0E,

            Terminator = 0xFF,
        }

        public class TileGraphicsInfo : R1Serializable
        {
            public byte TileIndex { get; set; }
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
                    bitFunc(default, 1, name: "Padding"); // Palette number for non-CGB mode, which is irrelevant here
                    Attr_HorizontalFlip = bitFunc(Attr_HorizontalFlip ? 1 : 0, 1, name: nameof(Attr_HorizontalFlip)) == 1;
                    Attr_VerticalFlip = bitFunc(Attr_VerticalFlip ? 1 : 0, 1, name: nameof(Attr_VerticalFlip)) == 1;
                    Attr_Prio = bitFunc(Attr_Prio ? 1 : 0, 1, name: nameof(Attr_Prio)) == 1;
                });
            }
        }
    }
}