using BinarySerializer;

namespace Ray1Map.GBARRR
{
    public class GBARRR_Mode7Object : BinarySerializable
    {
        public Mode7Type ObjectType { get; set; }
        public short XPosition { get; set; }
        public short YPosition { get; set; }
        public byte[] Data0 { get; set; }
        public short AnimFrame { get; set; }
        public byte[] Data1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectType = s.Serialize<Mode7Type>(ObjectType, name: nameof(ObjectType));
            if (ObjectType != Mode7Type.Invalid) {
                XPosition = s.Serialize<short>(XPosition, name: nameof(XPosition));
                YPosition = s.Serialize<short>(YPosition, name: nameof(YPosition));
                Data0 = s.SerializeArray<byte>(Data0, 10, name: nameof(Data0));
                AnimFrame = s.Serialize<short>(AnimFrame, name: nameof(AnimFrame));
                Data1 = s.SerializeArray<byte>(Data1, 14, name: nameof(Data1));
            }
        }

        public enum Mode7Type : short
        {
            Rayman = 0,
            Shadow = 1,
            Font = 2,
            RedLum = 3,
            YellowLum = 4,
            StopSign = 5,
            UIRayman = 6,
            UIHealthBar = 7,
            UIx = 8,
            UINumber = 9,
            UnknownLum = 10,
            UIFinishSign = 11,
            UILumCount = 12,
            UITotalLumCount = 13,
            UILapNumber = 14,
            UICountdown = 15,
            UnknownLum2 = 16,
            VehicleFlame = 17,
            RollingBall_Toy = 18,
            MenuRabbid = 19,
            MenuSelection = 20,
            MenuLogo = 21,
            MenuFlags = 22,
            SelectionSquare = 23,
            MenuCarrot = 24,
            Effect = 25,
            MenuCarrotVertical = 26,
            RollingBall_Cave = 27,
            RollingBall_Dessert = 28,
            Scenery_Cube = 29,
            Scenery_Pencil = 30,
            Scenery_PaperParasol = 31,
            Scenery_Cave = 32,
            UITimer = 37,
            UISwitch = 38,
            UISwitchOnOff = 39,
            GameOverRayman = 40,
            GameOverContinue = 41,
            UIGreenRedLight = 42,
            PauseMenuScreen = 43,
            MenuCarrotPause = 44,
            UnknownGraphic = 45,
            Invalid = 0xFE,
            Hidden = 0xFF
        }
    }
}