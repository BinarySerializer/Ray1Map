using System;

namespace R1Engine
{
    public class R1_PS1_FileTableEntry : R1Serializable
    {
        public Pointer FilePathPointer { get; set; }
        public uint MemoryAddress { get; set; } // Retail units have access to memory ranges between 0x80000000 and 0x801FFFFF
        public uint DevMemoryAddress { get; set; } // Dev units have access to memory ranges between 0x80000000 and 0x807FFFFF
        public byte DiscMinute { get; set; }
        public int DiscMinuteValue
        {
            get => FromBCD(DiscMinute);
            set => DiscMinute = ToBCD(value);
        }

        public byte DiscSecond { get; set; }
        public int DiscSecondValue
        {
            get => FromBCD(DiscSecond);
            set => DiscSecond = ToBCD(value);
        }

        public byte DiscFrame { get; set; }
        public int DiscFrameValue
        {
            get => FromBCD(DiscFrame);
            set => DiscFrame = ToBCD(value);
        }

        public uint FileSize { get; set; }
        public string FileName { get; set; }

        public string FilePath { get; set; }
        public string ProcessedFilePath { get; set; }

        public int LBA
        {
            get => (DiscFrameValue + (DiscSecondValue * 75) + (DiscMinuteValue * 60 * 75)) - 150;
            set
            {
                var tmp = value + 150;
                DiscFrameValue = (byte)(tmp % 75);
                DiscSecondValue = (byte)((tmp / 75) % 60);
                DiscMinuteValue = (byte)(tmp / 75 / 60);
            }
        }

        protected int FromBCD(byte bcd)
        {
            var result = 0;
            result *= 100;
            result += (10 * (bcd >> 4));
            result += bcd & 0xf;
            return result;
        }

        protected byte ToBCD(int value)
        {
            int tens = value / 10;
            int units = value % 10;
            return (byte)((tens << 4) | units);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 || s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
            {
                FilePath = s.SerializeString(FilePath, 32, name: nameof(FilePath));
                MemoryAddress = s.Serialize<uint>(MemoryAddress, name: nameof(MemoryAddress));
                DevMemoryAddress = s.Serialize<uint>(DevMemoryAddress, name: nameof(DevMemoryAddress));
                DiscMinute = s.Serialize<byte>(DiscMinute, name: nameof(DiscMinute));
                DiscSecond = s.Serialize<byte>(DiscSecond, name: nameof(DiscSecond));
                DiscFrame = s.Serialize<byte>(DiscFrame, name: nameof(DiscFrame));
                s.Serialize<byte>(default, name: "Padding");
                s.Log($"LBA: {LBA}");
                FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
                s.SerializeArray<byte>(new byte[s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 ? 12 : 8], s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 ? 12 : 8, name: "Padding");
            }
            else
            {
                FilePathPointer = s.SerializePointer(FilePathPointer, name: nameof(FilePathPointer));
                MemoryAddress = s.Serialize<uint>(MemoryAddress, name: nameof(MemoryAddress));
                DevMemoryAddress = s.Serialize<uint>(DevMemoryAddress, name: nameof(DevMemoryAddress));
                DiscMinute = s.Serialize<byte>(DiscMinute, name: nameof(DiscMinute));
                DiscSecond = s.Serialize<byte>(DiscSecond, name: nameof(DiscSecond));
                DiscFrame = s.Serialize<byte>(DiscFrame, name: nameof(DiscFrame));
                s.Serialize<byte>(default, name: "Padding");
                s.Log($"LBA: {LBA}");
                FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
                FileName = s.SerializeString(FileName, 16, name: nameof(FileName));

                FilePath = s.DoAt(FilePathPointer, () => s.SerializeString(FilePath, name: nameof(FilePath)));
            }

            ProcessedFilePath = FilePath.Replace('\\', '/').Replace(";1", "").TrimStart('/');

            if (s.GameSettings.GameModeSelection == GameModeSelection.RaymanPS1EUDemo || 
                s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                ProcessedFilePath = ProcessedFilePath.Replace("RAY/", "");
        }
    }
}