using System.Text;

namespace R1Engine
{
    /// <summary>
    /// Base data for a GBA ROM
    /// </summary>
    public class GBA_ROMBase : R1Serializable
    {
        public byte[] EntryPoint { get; set; }

        /// <summary>
        /// The Nintendo logo, compressed with Huffman
        /// </summary>
        public byte[] NintendoLogo { get; set; }

        public string GameTitle { get; set; }
        public string GameCode { get; set; }
        public string MakerCode { get; set; }

        /// <summary>
        /// Always 0x96
        /// </summary>
        public byte FixedValue { get; set; }

        public byte MainUnitCode { get; set; }
        public byte DeviceType { get; set; }

        /// <summary>
        /// Reserved unused bytes always 0x00
        /// </summary>
        public byte[] Reserved1 { get; set; }

        public byte SoftwareVersion { get; set; }
        
        /// <summary>
        /// The checksum for the header
        /// </summary>
        public byte ComplementCheck { get; set; }

        /// <summary>
        /// Reserved unused bytes always 0x00
        /// </summary>
        public byte[] Reserved2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            EntryPoint = s.SerializeArray<byte>(EntryPoint, 4, name: nameof(EntryPoint));
            NintendoLogo = s.SerializeArray<byte>(NintendoLogo, 156, name: nameof(NintendoLogo));
            GameTitle = s.SerializeString(GameTitle, 12, Encoding.ASCII, name: nameof(GameTitle));
            GameCode = s.SerializeString(GameCode, 4, Encoding.ASCII, name: nameof(GameCode));
            MakerCode = s.SerializeString(MakerCode, 2, Encoding.ASCII, name: nameof(MakerCode));
            FixedValue = s.Serialize<byte>(FixedValue, name: nameof(FixedValue));
            MainUnitCode = s.Serialize<byte>(MainUnitCode, name: nameof(MainUnitCode));
            DeviceType = s.Serialize<byte>(DeviceType, name: nameof(DeviceType));
            Reserved1 = s.SerializeArray<byte>(Reserved1, 7, name: nameof(Reserved1));
            SoftwareVersion = s.Serialize<byte>(SoftwareVersion, name: nameof(SoftwareVersion));
            ComplementCheck = s.Serialize<byte>(ComplementCheck, name: nameof(ComplementCheck));
            Reserved2 = s.SerializeArray<byte>(Reserved2, 2, name: nameof(Reserved2));
        }
    }
}