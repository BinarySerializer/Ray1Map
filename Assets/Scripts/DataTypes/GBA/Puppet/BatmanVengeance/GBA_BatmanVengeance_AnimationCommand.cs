using System;

namespace R1Engine
{
    public class GBA_BatmanVengeance_AnimationCommand : R1Serializable {
        #region Data
        public GBA_BatmanVengeance_Puppet Puppet { get; set; } // Set in onPreSerialize

        public InstructionCommand Command { get; set; }
        public byte CommandSize { get; set; }

        public byte Time { get; set; }

        public byte LayerCount { get; set; }

        public sbyte HitboxXPos { get; set; }
        public sbyte HitboxYPos { get; set; }
        public ushort HitboxHalfWidth { get; set; }
        public ushort HitboxHalfHeight { get; set; }

        public byte Byte_81_02 { get; set; }
        public byte Byte_81_03 { get; set; }
        public byte Byte_81_04 { get; set; }

        public byte Byte_82_02 { get; set; }
        public byte Byte_82_03 { get; set; }

        public byte Byte_83_02 { get; set; }
        public byte Byte_83_03 { get; set; }

        public sbyte Hitbox_Y1 { get; set; }
        public sbyte Hitbox_X1 { get; set; }
        public sbyte Hitbox_Y2 { get; set; }
        public sbyte Hitbox_X2 { get; set; }

        public byte Byte_85_02 { get; set; }
        public byte Byte_85_03 { get; set; }

        public byte Byte_86_02 { get; set; }
        public byte Byte_86_03 { get; set; }
        public byte Byte_86_04 { get; set; }
        public byte Byte_86_05 { get; set; }
        public byte Byte_86_06 { get; set; }
        public byte Byte_86_07 { get; set; }

        public TileGraphicsInfo[] TileMap { get; set; }
        public byte[] Padding { get; set; }

        #endregion

        #region Parsed

        public GBA_BatmanVengeance_AnimationChannel[] Layers { get; set; }

        public bool IsTerminator => Command == InstructionCommand.Terminator0 || Command == InstructionCommand.Terminator20;

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) {
            bool isShanghai = s.GameSettings.GBA_IsShanghai || s.GameSettings.GBA_IsMilan;
            Command = s.Serialize<InstructionCommand>(Command, name: nameof(Command));

            if (isShanghai)
                CommandSize = s.Serialize<byte>(CommandSize, name: nameof(CommandSize)); // In bytes

            switch (Command) {
                case InstructionCommand.SpriteNew:
                    LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 1 : 2, name: nameof(Padding));
                    Layers = s.SerializeObjectArray<GBA_BatmanVengeance_AnimationChannel>(Layers, LayerCount, name: nameof(Layers));
                    break;
                case InstructionCommand.SpriteTilemap:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    TileMap = s.SerializeObjectArray<TileGraphicsInfo>(TileMap, Puppet.TilemapWidth * Puppet.TilemapHeight, name: nameof(TileMap));
                    break;
                case InstructionCommand.Hitbox:
                    HitboxXPos = s.Serialize<sbyte>(HitboxXPos, name: nameof(HitboxXPos));
                    HitboxYPos = s.Serialize<sbyte>(HitboxYPos, name: nameof(HitboxYPos));
                    HitboxHalfWidth = s.Serialize<ushort>(HitboxHalfWidth, name: nameof(HitboxHalfWidth));
                    HitboxHalfHeight = s.Serialize<ushort>(HitboxHalfHeight, name: nameof(HitboxHalfHeight));
                    break;
                case InstructionCommand.Unknown81:
                    Byte_81_02 = s.Serialize<byte>(Byte_81_02, name: nameof(Byte_81_02));
                    Byte_81_03 = s.Serialize<byte>(Byte_81_03, name: nameof(Byte_81_03));
                    Byte_81_04 = s.Serialize<byte>(Byte_81_04, name: nameof(Byte_81_04));
                    break;
                case InstructionCommand.Unknown82:
                    Byte_82_02 = s.Serialize<byte>(Byte_82_02, name: nameof(Byte_82_02));
                    Byte_82_03 = s.Serialize<byte>(Byte_82_03, name: nameof(Byte_82_03));
                    break;
                case InstructionCommand.Unknown83:
                    Byte_83_02 = s.Serialize<byte>(Byte_83_02, name: nameof(Byte_83_02));
                    Byte_83_03 = s.Serialize<byte>(Byte_83_03, name: nameof(Byte_83_03));
                    break;
                case InstructionCommand.Hitbox_Batman:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    Hitbox_Y1 = s.Serialize<sbyte>(Hitbox_Y1, name: nameof(Hitbox_Y1));
                    Hitbox_X1 = s.Serialize<sbyte>(Hitbox_X1, name: nameof(Hitbox_X1));
                    Hitbox_Y2 = s.Serialize<sbyte>(Hitbox_Y2, name: nameof(Hitbox_Y2));
                    Hitbox_X2 = s.Serialize<sbyte>(Hitbox_X2, name: nameof(Hitbox_X2));
                    break;
                case InstructionCommand.Unknown85:
                    Byte_85_02 = s.Serialize<byte>(Byte_85_02, name: nameof(Byte_85_02));
                    Byte_85_03 = s.Serialize<byte>(Byte_85_03, name: nameof(Byte_85_03));
                    break;
                case InstructionCommand.Unknown86:
                    Byte_86_02 = s.Serialize<byte>(Byte_86_02, name: nameof(Byte_86_02));
                    Byte_86_03 = s.Serialize<byte>(Byte_86_03, name: nameof(Byte_86_03));
                    Byte_86_04 = s.Serialize<byte>(Byte_86_04, name: nameof(Byte_86_04));
                    Byte_86_05 = s.Serialize<byte>(Byte_86_05, name: nameof(Byte_86_05));
                    Byte_86_06 = s.Serialize<byte>(Byte_86_06, name: nameof(Byte_86_06));
                    Byte_86_07 = s.Serialize<byte>(Byte_86_07, name: nameof(Byte_86_07));
                    break;
                case InstructionCommand.Terminator0:
                case InstructionCommand.Terminator20:
                    Time = s.Serialize<byte>(Time, name: nameof(Time));
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 1 : 2, name: nameof(Padding));
                    break;
                default:
                    throw new Exception($"Unknown command {Command}");
            }
            if (isShanghai) 
            {
                if (CommandSize < 2) 
                    throw new Exception($"Command size {CommandSize} at {Offset}");

                s.Goto(Offset + CommandSize);
            }
        }

        #endregion



        public enum InstructionCommand : byte {
            Terminator0 = 0,
            SpriteNew = 0x01,
            SpriteTilemap = 0x10,
            Terminator20 = 0x20,
            Hitbox = 0x80,
            Unknown81 = 0x81,
            Unknown82 = 0x82,
            Unknown83 = 0x83,
            Hitbox_Batman = 0x84,
            Unknown85 = 0x85,
            Unknown86 = 0x86,
        }



        public class TileGraphicsInfo : R1Serializable {
            public ushort TileIndex { get; set; } // If -1, same effect as SetInvisible, otherwise SetVisible
            public bool IsFlippedHorizontally { get; set; }
            public bool IsFlippedVertically { get; set; }
            public byte PaletteIndex { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                s.SerializeBitValues<ushort>(bitFunc => {
                    TileIndex = (ushort)bitFunc(TileIndex, 10, name: nameof(TileIndex));
                    IsFlippedHorizontally = bitFunc(IsFlippedHorizontally ? 1 : 0, 1, name: nameof(IsFlippedHorizontally)) == 1;
                    IsFlippedVertically = bitFunc(IsFlippedVertically ? 1 : 0, 1, name: nameof(IsFlippedHorizontally)) == 1;
                    PaletteIndex = (byte)bitFunc(PaletteIndex, 4, name: nameof(PaletteIndex));
                });
            }
        }
    }
}