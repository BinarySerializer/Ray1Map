using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Base data for a GBC ROM
    /// </summary>
    public class GBC_ROMBase : R1Serializable
    {
        public byte[] EntryPoint { get; set; }
        public byte[] NintendoLogo { get; set; }
        public string GameTitle { get; set; }
        public string MaManufacturerCode { get; set; }
        public byte CGBFlag { get; set; }
        public ushort NewLicenseeCode { get; set; }
        public byte SGBFlag { get; set; }
        public Cartridge_Type CartridgeType { get; set; }
        public ROM_Size ROMSize { get; set; }
        public RAM_Size RAMSize { get; set; }
        public Destination_Code DestinationCode { get; set; }
        public byte OldLicenseeCode { get; set; }
        public byte GameVersion { get; set; }
        public byte HeaderChecksum { get; set; }
        public ushort GlobalChecksum { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // The header begins at 0x100
            s.DoAt(s.CurrentPointer + 0x100, () =>
            {
                EntryPoint = s.SerializeArray<byte>(EntryPoint, 4, name: nameof(EntryPoint));
                NintendoLogo = s.SerializeArray<byte>(NintendoLogo, 48, name: nameof(NintendoLogo));
                GameTitle = s.SerializeString(GameTitle, 11, Encoding.ASCII, name: nameof(GameTitle));
                MaManufacturerCode = s.SerializeString(MaManufacturerCode, 4, Encoding.ASCII, name: nameof(MaManufacturerCode));
                CGBFlag = s.Serialize<byte>(CGBFlag, name: nameof(CGBFlag));
                NewLicenseeCode = s.Serialize<ushort>(NewLicenseeCode, name: nameof(NewLicenseeCode));
                SGBFlag = s.Serialize<byte>(SGBFlag, name: nameof(SGBFlag));
                CartridgeType = s.Serialize<Cartridge_Type>(CartridgeType, name: nameof(CartridgeType));
                ROMSize = s.Serialize<ROM_Size>(ROMSize, name: nameof(ROMSize));
                RAMSize = s.Serialize<RAM_Size>(RAMSize, name: nameof(RAMSize));
                DestinationCode = s.Serialize<Destination_Code>(DestinationCode, name: nameof(DestinationCode));
                OldLicenseeCode = s.Serialize<byte>(OldLicenseeCode, name: nameof(OldLicenseeCode));
                GameVersion = s.Serialize<byte>(GameVersion, name: nameof(GameVersion));
                HeaderChecksum = s.Serialize<byte>(HeaderChecksum, name: nameof(HeaderChecksum));
                GlobalChecksum = s.Serialize<ushort>(GlobalChecksum, name: nameof(GlobalChecksum));
            });
        }

        public enum Cartridge_Type : byte
        {
            ROMONLY = 0x00,
            MBC1 = 0x01,
            MBC1_RAM = 0x02,
            MBC1_RAM_BATTERY = 0x03,
            MBC2 = 0x05,
            MBC2_BATTERY = 0x06,
            ROM_RAM = 0x08,
            ROM_RAM_BATTERY = 0x09,
            MMM01 = 0x0B,
            MMM01_RAM = 0x0C,
            MMM01_RAM_BATTERY = 0x0D,
            MBC3_TIMER_BATTERY = 0x0F,
            MBC3_TIMER_RAM_BATTERY = 0x10,
            MBC3 = 0x11,
            MBC3_RAM = 0x12,
            MBC3_RAM_BATTERY = 0x13,
            MBC5 = 0x19,
            MBC5_RAM = 0x1A,
            MBC5_RAM_BATTERY = 0x1B,
            MBC5_RUMBLE = 0x1C,
            MBC5_RUMBLE_RAM = 0x1D,
            MBC5_RUMBLE_RAM_BATTERY = 0x1E,
            MBC6 = 0x20,
            MBC7_SENSOR_RUMBLE_RAM_BATTERY = 0x22,
            POCKET_CAMERA = 0xFC,
            BANDAI_TAMA5 = 0xFD,
            HuC3 = 0xFE,
            HuC1_RAM_BATTER = 0xFF,
        }
        public enum ROM_Size : byte
        {
            Size_32KB = 0x00, // 0 banks
            Size_64KB = 0x01, // 4 banks
            Size_128KB = 0x02, // 8 banks
            Size_256KB = 0x03, // 16 banks
            Size_512KB = 0x04, // 32 banks
            Size_1MB = 0x05, // 64 banks (only 63 banks used by MBC1)
            Size_2MB = 0x06, // 128 banks (only 125 banks used by MBC1)
            Size_4MB = 0x07, // 256 banks
            Size_8MB = 0x08, // 512 banks
            Size_1_1MB = 0x52, // 72 banks
            Size_1_2MB = 0x53, // 80 banks
            Size_1_5MB = 0x54, // 96 banks
        }
        public enum RAM_Size : byte
        {
            None = 0x00,
            Size_2KB = 0x01,
            Size_8KB = 0x02,
            Size_32KB = 0x03, // 4 banks of 8KBytes each
            Size_128KB = 0x04, // 16  banks of 8KBytes each
            Size_64KB = 0x05, // 8  banks of 8KBytes each
        }
        public enum Destination_Code : byte
        {
            Japanese = 0x00,
            NonJapanese = 0x01,
        }
    }
}