namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class PaletteFile : OnyxFile
    {
        public uint DataLength { get; set; }
        public RGB555Color[] ColorData { get; set; }

        public override void SerializeFile(SerializerObject s)
        {
            DataLength = s.Serialize<uint>(DataLength, name: nameof(DataLength));
            ColorData = s.SerializeObjectArray<RGB555Color>(ColorData, DataLength / 2, name: nameof(ColorData));
        }
    }
}