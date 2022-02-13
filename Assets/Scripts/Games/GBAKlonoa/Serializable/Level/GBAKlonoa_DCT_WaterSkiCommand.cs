using System;
using BinarySerializer;

namespace Ray1Map.GBAKlonoa
{
    public class GBAKlonoa_DCT_WaterSkiCommand : BinarySerializable
    {
        public WaterSkiCommandType CmdType { get; set; }
        
        public byte ObjIndex { get; set; }
        public short XPos { get; set; }
        public short YPos { get; set; }
        public short ZPos { get; set; }
        public byte ObjValue_1D { get; set; }

        public ushort Ushort_00 { get; set; }
        public ushort Ushort_02 { get; set; }

        public short Klonoa_YPos { get; set; }
        
        public ushort CMDOffset { get; set; }

        public byte BG { get; set; }

        public short AnimInstanceIndex { get; set; }
        public byte AnimIndex { get; set; }
        
        public override void SerializeImpl(SerializerObject s)
        {
            CmdType = s.Serialize<WaterSkiCommandType>(CmdType, name: nameof(CmdType));

            switch (CmdType)
            {
                case WaterSkiCommandType.Object_0:
                case WaterSkiCommandType.Object_1:

                    // Note: If Object_1 then the XPos gets offset by the current waterski XPos

                    ObjIndex = s.Serialize<byte>(ObjIndex, name: nameof(ObjIndex));
                    s.SerializePadding(1, logIfNotNull: true);
                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                    ZPos = s.Serialize<short>(ZPos, name: nameof(ZPos));
                    ObjValue_1D = s.Serialize<byte>(ObjValue_1D, name: nameof(ObjValue_1D));
                    s.SerializePadding(1, logIfNotNull: true);

                    break;
                
                case WaterSkiCommandType.CMD_01:

                    Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                    Klonoa_YPos = s.Serialize<short>(Klonoa_YPos, name: nameof(Klonoa_YPos));

                    break;

                case WaterSkiCommandType.ChangeDirection:

                    Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));

                    if (Ushort_00 * 0x1000000 < 1)
                    {
                        // This changes the animations of Klonoa facing him towards the camera
                    }
                    
                    break;

                case WaterSkiCommandType.EndOfSector:

                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));

                    break;

                case WaterSkiCommandType.CMD_04:
                case WaterSkiCommandType.CMD_05:

                    // Conditionally end sector

                    break;

                case WaterSkiCommandType.GoTo:

                    CMDOffset = s.Serialize<ushort>(CMDOffset, name: nameof(CMDOffset));
                    throw new NotImplementedException("GOTO is not currently supported");

                case WaterSkiCommandType.CMD_09:
                    // Set 0x03005cf0 to 0
                    break;

                case WaterSkiCommandType.CMD_0A:

                    // End read if 0x03005cf0 <= Ushort_00
                    Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                    
                    break;

                case WaterSkiCommandType.CMD_0B:
                    
                    // ?
                    
                    break;

                case WaterSkiCommandType.CMD_0C:
                case WaterSkiCommandType.CMD_0D:

                    // Do nothing...

                    break;

                case WaterSkiCommandType.CMD_0E:

                    Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));

                    break;

                case WaterSkiCommandType.CMD_0F:

                    Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));

                    break;

                case WaterSkiCommandType.EnableBG:

                    BG = s.Serialize<byte>(BG, name: nameof(BG));
                    s.SerializePadding(1, logIfNotNull: true);

                    break;

                case WaterSkiCommandType.DisableBG:

                    BG = s.Serialize<byte>(BG, name: nameof(BG));
                    s.SerializePadding(1, logIfNotNull: true);

                    break;

                case WaterSkiCommandType.CMD_13:
                case WaterSkiCommandType.CMD_14:

                    AnimInstanceIndex = s.Serialize<short>(AnimInstanceIndex, name: nameof(AnimInstanceIndex));
                    Ushort_02 = s.Serialize<ushort>(Ushort_02, name: nameof(Ushort_02));

                    break;

                case WaterSkiCommandType.SetAnimIndex:

                    AnimInstanceIndex = s.Serialize<short>(AnimInstanceIndex, name: nameof(AnimInstanceIndex));
                    AnimIndex = s.Serialize<byte>(AnimIndex, name: nameof(AnimIndex));
                    s.SerializePadding(1, logIfNotNull: true);

                    break;

                case WaterSkiCommandType.EndOfLevel:

                    // Do nothing...

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(CmdType), CmdType, null);
            }
        }

        public enum WaterSkiCommandType : ushort
        {
            Object_0 = 0x00,
            CMD_01 = 0x01,
            ChangeDirection = 0x02,
            EndOfSector = 0x03,
            CMD_04 = 0x04,
            CMD_05 = 0x05,
            GoTo = 0x06,
            CMD_09 = 0x09,
            CMD_0A = 0x0A,
            CMD_0B = 0x0B,
            CMD_0C = 0x0C,
            CMD_0D = 0x0D,
            CMD_0E = 0x0E,
            CMD_0F = 0x0F,
            EnableBG = 0x10,
            DisableBG = 0x11,
            Object_1 = 0x12,
            CMD_13 = 0x13,
            CMD_14 = 0x14,
            SetAnimIndex = 0x15,
            EndOfLevel = 0x63,
        }
    }
}