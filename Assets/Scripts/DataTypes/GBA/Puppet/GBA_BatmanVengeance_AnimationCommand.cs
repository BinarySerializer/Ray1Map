using System;
using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_BatmanVengeance_AnimationCommand : R1Serializable {
        #region Data
        public GBA_BatmanVengeance_Puppet Puppet { get; set; } // Set in onPreSerialize

        public InstructionCommand Command { get; set; }
        public byte CommandSize { get; set; }


        public byte LayerCount { get; set; }

        public byte Byte_80_02 { get; set; }
        public byte Byte_80_03 { get; set; }
        public byte Byte_80_04 { get; set; }
        public byte Byte_80_05 { get; set; }
        public byte Byte_80_06 { get; set; }
        public byte Byte_80_07 { get; set; }

        public byte Byte_83_02 { get; set; }
        public byte Byte_83_03 { get; set; }

        public byte Byte_84_04 { get; set; }
        public byte Byte_84_05 { get; set; }
        public byte Byte_84_06 { get; set; }
        public byte Byte_84_07 { get; set; }

        public ushort[] TileMapIndices { get; set; }
        public byte[] Padding { get; set; }

        #endregion

        #region Parsed

        public GBA_BatmanVengeance_AnimationChannel[] Layers { get; set; }

        public bool IsTerminator => Command == InstructionCommand.Terminator0 || Command == InstructionCommand.Terminator20;

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) {
            bool isShanghai = s.GameSettings.EngineVersion < EngineVersion.GBA_BatmanVengeance;
            Command = s.Serialize<InstructionCommand>(Command, name: nameof(Command));
            if (isShanghai) {
                CommandSize = s.Serialize<byte>(CommandSize, name: nameof(CommandSize)); // In bytes
            }
            switch (Command) {
                case InstructionCommand.SpriteNew:
                    LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 1 : 2, name: nameof(Padding));
                    Layers = s.SerializeObjectArray<GBA_BatmanVengeance_AnimationChannel>(Layers, LayerCount, name: nameof(Layers));
                    break;
                case InstructionCommand.SpriteTilemap:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    TileMapIndices = s.SerializeArray<ushort>(TileMapIndices, Puppet.TilemapWidth * Puppet.TilemapHeight, name: nameof(TileMapIndices));
                    break;
                case InstructionCommand.Unknown80:
                    Byte_80_02 = s.Serialize<byte>(Byte_80_02, name: nameof(Byte_80_02));
                    Byte_80_03 = s.Serialize<byte>(Byte_80_03, name: nameof(Byte_80_03));
                    Byte_80_04 = s.Serialize<byte>(Byte_80_04, name: nameof(Byte_80_04));
                    Byte_80_05 = s.Serialize<byte>(Byte_80_05, name: nameof(Byte_80_05));
                    Byte_80_06 = s.Serialize<byte>(Byte_80_06, name: nameof(Byte_80_06));
                    Byte_80_07 = s.Serialize<byte>(Byte_80_07, name: nameof(Byte_80_07));
                    break;
                case InstructionCommand.Unknown83:
                    Byte_83_02 = s.Serialize<byte>(Byte_83_02, name: nameof(Byte_83_02));
                    Byte_83_03 = s.Serialize<byte>(Byte_83_03, name: nameof(Byte_83_03));
                    break;
                case InstructionCommand.Unknown84:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    Byte_84_04 = s.Serialize<byte>(Byte_84_04, name: nameof(Byte_84_04));
                    Byte_84_05 = s.Serialize<byte>(Byte_84_05, name: nameof(Byte_84_05));
                    Byte_84_06 = s.Serialize<byte>(Byte_84_06, name: nameof(Byte_84_06));
                    Byte_84_07 = s.Serialize<byte>(Byte_84_07, name: nameof(Byte_84_07));
                    break;
                case InstructionCommand.Terminator0:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    break;
                case InstructionCommand.Terminator20:
                    Padding = s.SerializeArray<byte>(Padding, isShanghai ? 2 : 3, name: nameof(Padding));
                    break;
            }
            if (isShanghai) {
                if(CommandSize < 2) throw new Exception($"Command size {CommandSize} at {Offset}");
                s.Goto(Offset + CommandSize);
            }
        }

        #endregion



        public enum InstructionCommand : byte {
            Terminator0 = 0,
            SpriteNew = 0x01,
            SpriteTilemap = 0x10,
            Terminator20 = 0x20,
            Unknown80 = 0x80,
            Unknown83 = 0x83,
            Unknown84 = 0x84,
        }
    }
}